using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using static Bud;

/// <summary>
/// Tree seeds are items that are activated on interaction. They grow L-System branches. These branches have the spawn buds of the selected fruit type.
/// </summary>
public class TreeSeed : Item
{
    [Header("References and prefabs")]
    public GameObject budPrefab;
    [Header("General Settings")]
    public FruitType fruitType;
    public float sproutTime;
    public float growTime;
    public Color baseColor;
    public float minDistanceToOtherPlants;
    [Header("L-System and Shape Settings")]
    public  List<GameObject> branchPrefabs;
    public GameObject leafPrefab;
    public int productionIterations;
    public string originalString;
    public List<string> productions;
    public float branchAngleIncrement;
    public float branchSizeDecrement;
    string generationString;
    TurtleState currentTurtleState;
    Stack<TurtleState> turtleStack;
    List<(GameObject, float)> branches;
    List<Bud> allBuds;



    // Runtime variables
    float currentGrowTime;
    float currentSproutTime;
    bool isGrowing;
    bool isSprouting;
    Transform currentBubble;

    public class TurtleState{
        public Transform branchEnd;
        public float angle;
        public int branchOrder;
        public TurtleState(Transform branchEnd, float angle, int branchOrder){
            this.branchEnd = branchEnd;
            this.angle = angle;
            this.branchOrder = branchOrder;
        }
    }
    public override void Start()
    {
        base.Start();
        branches = new List<(GameObject, float)>();
        turtleStack = new Stack<TurtleState>();
        allBuds = new List<Bud>();
    }

    void Update()
    {
        if(isGrowing){
            if(currentGrowTime >= growTime){
                // Plant reaches maturity. This is only called one time. TODO: Some bloom logic + particle effects + animation
                isGrowing = false;
            }
            currentGrowTime += Time.deltaTime;
        }

        if(!isGrowing && currentGrowTime >= growTime){
            // Plant is mature
            currentGrowTime += Time.deltaTime; // This is basically the plants age counter from this point on
            
        }
        if(isSprouting){
            currentSproutTime += Time.deltaTime;
            if(currentSproutTime >= sproutTime){
                // Sprout time over. Seed shall grow to plant now.
                // Do various checks first. Then grow.
                bool canGrow = false;
                if(currentBubble != null){
                    Vector2 bubbleDirection = (currentBubble.transform.position - transform.position).normalized;
                    RaycastHit2D bubbleHit = Physics2D.Raycast(transform.position, bubbleDirection, 1f, LayerMask.GetMask("Ground"));
                    if(bubbleHit.collider != null){
                        // Close enough to bubble. Move as close as possible.
                        transform.position = new Vector3(bubbleHit.point.x, bubbleHit.point.y, transform.position.z);
                        canGrow = true;
                    }
                }
                if(canGrow){
                    Grow();
                    isSprouting = false;
                }
            }
        }
    }
    /// <summary>
    /// Starts an L-System algorithm that generates branches and leaves.
    /// </summary>
    protected virtual void Grow(){
        isGrowing = true;
        if(currentBubble != null){
            // Is on bubble. Start growth.
            GetComponent<CustomGravity>().enabled = false;
            GetComponent<Rigidbody2D>().simulated = false;
            Vector2 bubbleDirection = (currentBubble.transform.position - transform.position).normalized;
            Vector2 target = new Vector2(transform.position.x, transform.position.y) - bubbleDirection;
            transform.up = (Vector3)target - transform.position;
            // Generate production string
            generationString = originalString;
            for(int i = 0; i < productionIterations; i++){
                string iterationString = "";
                for(int j = 0; j < generationString.Length; j++){
                    if(generationString[j] == 'F'){
                        // Replace with production
                        string productionString = productions[Random.Range(0, productions.Count)];
                        iterationString += productionString;
                    }
                    else{
                        iterationString += generationString[j];
                    }
                }
                generationString = iterationString;
            }
            Debug.Log(generationString);

            // Generate tree
            currentTurtleState = new TurtleState(null, 0f, 1);
            foreach(char c in generationString){
                switch(c){
                    case 'F':{
                        // Generate branch, rotate by current rotation, set new branch end and update state
                        GameObject nextBranch = currentTurtleState.branchEnd == null ? Instantiate(branchPrefabs[Random.Range(0, branchPrefabs.Count)], transform) : Instantiate(branchPrefabs[Random.Range(0, branchPrefabs.Count)], currentTurtleState.branchEnd);
                        nextBranch.transform.Rotate(Vector3.forward, currentTurtleState.angle);
                        nextBranch.transform.localScale = Vector3.one; //* (branchSizeDecrement / currentTurtleState.branchOrder);
                        currentTurtleState = new TurtleState(nextBranch.transform.GetChild(0), currentTurtleState.angle, currentTurtleState.branchOrder);

                        // TODO: Generate leaves if branchOrder is high enough
                        if(currentTurtleState.branchOrder > 1){
                            Bud bud = Instantiate(budPrefab, nextBranch.transform.Find("BranchEnd")).GetComponent<Bud>();
                            bud.SetType(fruitType);
                            allBuds.Add(bud);
                        }
                        break;
                    }
                    case '[':{
                        turtleStack.Push(new TurtleState(currentTurtleState.branchEnd, currentTurtleState.angle, currentTurtleState.branchOrder));
                        currentTurtleState.branchOrder++;
                        break;
                    }
                    case ']':{
                        currentTurtleState = turtleStack.Pop();
                        break;
                    }
                    case '+':{
                        currentTurtleState.angle += branchAngleIncrement;
                        break;
                    }
                    case '-':{
                        currentTurtleState.angle -= branchAngleIncrement;
                        break;
                    }
                }
            }
            turtleStack.Clear();
        }
        else{
        }
 
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Bubble")){
            currentBubble = other.transform;
        }
    }
    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Bubble")){
            currentBubble = null;
        }
    }

    public override void Throw(Vector2 throwDirection, float throwForce)
    {
        //TODO: When adding particles, do extra particles for throwing here.
        base.Throw(throwDirection, throwForce);
        TryActivate();
    }

    public override bool TryActivate()
    {
        if(isInteractable){
            isSprouting = true;
            return true;
        }
        else{
            return false;
        }
    }

    public void Shake(){
        foreach(Bud bud in allBuds){ // Drop all fruits on buds
            if(bud.TryHarvest()){
                // TODO: Add harvest successful effect here
            }
            else{
                // TODO: Add harvest failed effect here
            }
        }
    }

}

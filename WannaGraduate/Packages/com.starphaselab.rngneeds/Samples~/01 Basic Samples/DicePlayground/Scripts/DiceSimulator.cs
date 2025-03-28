using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace RNGNeeds.Samples.DicePlayground
{
    public class DiceSimulator : MonoBehaviour
    {
        [Header("D6 Die")]
        public Die die;
        
        [Header("Setup")]
        [Range(1, 9)] public int numOfDice = 3;
        public Vector3 forceMin;
        public Vector3 forceMax;
        public Vector3 torqueMin;
        public Vector3 torqueMax;
        
        [Header("Scene Links")]
        public GameObject groundObject;
        public CameraController cameraController;
        public List<Transform> spawnPositions;
        public TMP_Text resultText;
        public TMP_Text buttonText;
        public Slider numOfDiceSlider;
        public Toggle maintainPickCountToggle;
        
        private bool playing;
        private int currentFrame;
        private int recordedFrames;
        private Scene m_SimulationScene;
        private PhysicsScene m_PhysicsScene;
        private readonly List<DieBrain> dieBrains = new List<DieBrain>();
        private readonly Dictionary<DieBrain, List<(Vector3 simPosition, Quaternion simRotation)>> simData = new Dictionary<DieBrain, List<(Vector3 simPosition, Quaternion simRotation)>>();
        
        private void Start()
        {
            groundObject.GetComponent<BoxCollider>().size = Vector3.one;
            CreateSimulationScene();
            SetNumOfDice();
        }

        private void Update()
        {
            maintainPickCountToggle.isOn = die.sides.MaintainPickCountIfDisabled;
        }

        private void CreateSimulationScene()
        {
            m_SimulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
            m_PhysicsScene = m_SimulationScene.GetPhysicsScene();
            SceneManager.MoveGameObjectToScene(groundObject, m_SimulationScene);
        }
        
        public void Roll()
        {
            resultText.text = "";
            currentFrame = 0;
            recordedFrames = 0;
            Simulate();
            playing = true;
        }

        private void Simulate()
        {
            // Destroy any existing dice
            foreach (var simDieBrain in dieBrains) if(simDieBrain != null) Destroy(simDieBrain.gameObject);
            dieBrains.Clear();
            simData.Clear();
            
            // Create Dice and move them to simulation scene
            for (var i = 1; i <= numOfDice; i++)
            {
                var rollResult = die.Roll(1);   // RNDNeeds - pick value from probability list
                if(rollResult < 1) continue;
                resultText.text += $"{rollResult.ToString()}  ";
                var dieObject = Instantiate(die.diePrefab, spawnPositions[i - 1].position, Quaternion.identity);
                var simBrain = dieObject.GetComponent<DieBrain>();
                simBrain.rollResult = rollResult;
                simBrain.SetForceAndTorque(Vector3.Lerp(forceMin, forceMax, Random.Range(0f, 1f)), Vector3.Lerp(torqueMin, torqueMax, Random.Range(0f, 1f)));
                simBrain.spawnPosition = spawnPositions[i - 1].position;
                SceneManager.MoveGameObjectToScene(dieObject, m_SimulationScene);
                dieBrains.Add(simBrain);
            }

            // Add Force and Torque to dice
            foreach (var simDieBrain in dieBrains) simDieBrain.PerformRoll();

            while (true)
            {
                // Step through simulation
                m_PhysicsScene.Simulate(Time.fixedDeltaTime);
                
                // Record position and rotation of each die
                foreach (var simDieBrain in dieBrains) Record(simDieBrain);
                recordedFrames++;
                
                // Check if any rigidbody is still in motion
                var allSleeping = true;
                foreach (var simDieBrain in dieBrains)
                {
                    if (simDieBrain.objectBody.IsSleeping()) continue;
                    allSleeping = false;
                    break;
                }

                // If all rigidbodies are resting, break from simulation loop
                if (allSleeping) break;
            }

            var geometryCenter = Vector3.zero;
            foreach (var simDieBrain in dieBrains)
            {
                geometryCenter += simDieBrain.gameObject.transform.position;
                simDieBrain.DetermineTopFace();
                simDieBrain.dieCollider.enabled = false;
                simDieBrain.objectBody.useGravity = false;
                simDieBrain.ResetAndOrient();
            }

            geometryCenter /= dieBrains.Count;
            if(float.IsNaN(geometryCenter.x) == false) cameraController.SetNewTarget(geometryCenter, dieBrains.Count);
        }

        private void FixedUpdate()
        {
            if (playing == false) return;
            Play(currentFrame);
            currentFrame++;
        }

        private void Play(int frame)
        {
            if (frame >= recordedFrames)
            {
                playing = false;
                return;
            }
            
            foreach (var data in simData) data.Key.transform.SetPositionAndRotation(data.Value[frame].simPosition, data.Value[frame].simRotation);
        }

        private void Record(DieBrain dieBrain)
        {
            if (simData.TryGetValue(dieBrain, out var _) == false) simData.Add(dieBrain, new List<(Vector3 simPosition, Quaternion simRotation)>());
            var dieTransform = dieBrain.transform;
            simData[dieBrain].Add((dieTransform.position, dieTransform.rotation));
        }

        public void SetNumOfDice()
        {
            numOfDice = (int)numOfDiceSlider.value;
            buttonText.text = numOfDice > 1 ? $"Roll {numOfDice} Dice" : $"Roll {numOfDice} Die";
        }

        public void SetMaintainPickCount()
        {
            die.sides.MaintainPickCountIfDisabled = maintainPickCountToggle.isOn;
        }
    }
}
// (c) Copyright 2020 HP Development Company, L.P.

using System;
using System.Collections;
using System.Collections.Generic;
using HP.Glia.Examples.Common;
using UnityEngine;
using UnityEngine.Events;

namespace HP.Glia.Examples.Follow
{
    public class Follow : MonoBehaviour
    {
        [Header("Scene references")]
        [SerializeField] public List<FollowButton> Buttons = new List<FollowButton>();
        [SerializeField] private TableLivesDisplay livesDisplay = null;
        [Space(5), SerializeField] private ButtonInteractable StartButton = null;
        public GameObject StartInstructions = null;

        
        [Header("Cognitive Load Graphs")]
        public CognitiveLoadGraph RoundCognitiveLoadGraph = null;
        public CognitiveLoadGraph ExperienceCognitiveLoadGraph = null;
        public SDGraph RoundSDGraph = null;


        [Header("Cognitive Load Monitors")]
        public CognitiveLoadMonitor RoundClMonitor;
        public CognitiveLoadMonitor ExperienceClMonitor;

        
        [Header("Confirguaration")]
        [SerializeField] protected float TimeToNextButton = 0.5f;
        public int Lives = 3;

        [SerializeField] protected float MaxButtonTimeOn = 2f;
        [SerializeField] protected float MinButtonTimeOn = 0.1f;

        [Space(10)]
        [Tooltip("Scale effects with remaining lives when round passed and with Lives - remaining lives when failed")]
        [SerializeField] private bool ScaleEffectsWithLives = true;

        [Header("Fail Round Time On Tweak")]
        [SerializeField] private float FailLowCLTimeOnEffect = 0;
        [SerializeField] private float FailMedCLTimeOnEffect = 0.05f;
        [SerializeField] private float FailHighCLTimeOnEffect = 0.1f;

        
        [Header("Correct Round Time On Tweak")]
        [SerializeField] private float CorrectLowCLTimeOnEffect = -0.225f;
        [SerializeField] private float CorrectMedCLTimeOnEffect = -0.125f;
        [SerializeField] private float CorrectHighCLTimeOnEffect = 0;

        [Header("Events")]
        
        public UnityEvent OnGameSatarted;
        public UnityEvent OnGameOver;
        public UnityEvent OnRoundStarted;
        public UnityEvent OnRoundFailed;
        public UnityEvent OnRoundSucceed;


        [Header("Debug")]
        [SerializeField] protected List<int> m_sequence;
        [SerializeField] protected GameState gameState = GameState.NotStarted;
        [SerializeField] private int m_remaningLives = 0;
        [SerializeField] protected int m_inputAction = 0;

        protected float m_timeOn = 1.25f;
        protected float m_timeToNextButton = 0.5f;




        void Start()
        {
            //Set different colors for the buttons using HSV space
            float increment = 1f/Buttons.Count;
            
            for (int i = 0; i < Buttons.Count; i++){
                UnityEngine.Color color = UnityEngine.Color.HSVToRGB(increment*i, 0.55f, 1f);
                
                Buttons[i].SetData(i, color);
                Buttons[i].gameObject.SetActive(false);
            }

            //Add listener to the start
            StartButton.OnButtonReleased.AddListener(StartGame);
        }

        [ContextMenu("Start game")]
        public virtual void StartGame(){
            AudioSource[] audioSources = GetComponents<AudioSource>();
            audioSources[0].Play();

            gameState = GameState.BetweenRounds;

            StartButton.gameObject.SetActive(false);
            StartInstructions.gameObject.SetActive(false);
            //Enable all buttons -> Show hide methods on buttons?
            float increment = 1f/Buttons.Count;
            
            for(int i = 0; i < Buttons.Count; i++){
                UnityEngine.Color color = UnityEngine.Color.HSVToRGB(increment*i, 0.55f, 1f);
                
                Buttons[i].SetData(i, color);
                Buttons[i].gameObject.SetActive(true);
            }
            
            
            OnGameSatarted.Invoke();

            m_sequence = new List<int>();
            m_remaningLives = Lives;
            livesDisplay.SetRemainingLives(m_remaningLives);
            m_timeOn = ((MaxButtonTimeOn - MinButtonTimeOn)/2f) + MinButtonTimeOn;
            m_timeToNextButton = TimeToNextButton;

            StartNewRound(2f);

            ExperienceClMonitor?.StartMonitoring();
        }

        protected virtual void StartNewRound(float initialWait =  1f){
            m_inputAction = 0;
            m_sequence.Add(UnityEngine.Random.Range(0, Buttons.Count));

            StartCoroutine(ShowSequence(m_sequence, initialWait));
            RoundClMonitor?.StartMonitoring();
            OnRoundStarted.Invoke();
        }

        protected IEnumerator ShowSequence(List<int> sequence, float initialWait = 0f){
            yield return new WaitForSeconds(initialWait);

            gameState = GameState.ShowingSequence;

            BlockInteraction();

            yield return new WaitForSeconds(1f);
            
            for(int i = 0; i < sequence.Count; i++){
                Buttons[sequence[i]].TurnOn();
                yield return new WaitForSeconds(m_timeOn);
                Buttons[sequence[i]].TurnOff();
                yield return new WaitForSeconds(m_timeToNextButton);
            }  

            gameState = GameState.UserPlayingSequence;
            AllowInteraction();      
        }

        protected virtual void RecordClick(int id)
        {
            if(gameState != GameState.UserPlayingSequence) return;

            //Correct
            if(m_sequence[m_inputAction] == id){
                //Next Round
                if(++m_inputAction >= m_sequence.Count){
                    OnRoundSucceed.Invoke();
                    NextRound();
                }
            }
            //Incorrect
            else{
                BlockInteraction();
                livesDisplay.SetRemainingLives(--m_remaningLives);
                m_inputAction = 0;
                OnRoundFailed.Invoke();

                //Try again
                if(m_remaningLives > 0){
                    Retry();
                }
                //Game Over
                else{
                   GameOver();
                }

            }
        }

        protected virtual void NextRound(){
            BlockInteraction();
            gameState = GameState.BetweenRounds;
            RoundClMonitor?.StopMonitoring();

            if(RoundClMonitor != null){
                RoundCognitiveLoadGraph?.ShowResult(true, RoundClMonitor);
                RoundSDGraph?.ShowResult(RoundClMonitor);
            }

            if(ExperienceClMonitor != null){
                ExperienceCognitiveLoadGraph?.ShowResult(ExperienceClMonitor);
            }

            CLAwareTweakParameters(true, RoundClMonitor.GetMean());
            StartNewRound();
        }

        protected virtual void Retry(){
            gameState = GameState.BetweenRounds;
            RoundClMonitor?.StopMonitoring();

            if(RoundClMonitor != null){
                RoundCognitiveLoadGraph?.ShowResult(false, RoundClMonitor);
                RoundSDGraph?.ShowResult(RoundClMonitor);
            }

            if(ExperienceClMonitor != null){
                ExperienceCognitiveLoadGraph?.ShowResult(ExperienceClMonitor);
            }
            
            CLAwareTweakParameters(false, RoundClMonitor != null ?  RoundClMonitor.GetMean() : 0.5f);
            StartCoroutine(ShowSequence(m_sequence, 2f));
            
            OnRoundStarted.Invoke();
        }

        protected virtual void GameOver(){
            gameState = GameState.NotStarted;
            StartButton.gameObject.SetActive(true);
            StartInstructions.gameObject.SetActive(true);
            for(int i = 0; i < Buttons.Count; i++){
                Buttons[i].gameObject.SetActive(false);
            }

            if(ExperienceClMonitor != null){
                ExperienceClMonitor?.StopMonitoring();
                ExperienceCognitiveLoadGraph?.ShowResult(ExperienceClMonitor);

            }

            if(RoundClMonitor != null){
                RoundSDGraph?.ShowResult(RoundClMonitor);
            }

            OnGameOver.Invoke();
        }

        protected virtual void CLAwareTweakParameters(bool correctRound, float clMean){
            // Reduce memorization time
            if(correctRound){
                if(clMean > 0.75){ // Correct Round but hard time with it
                    m_timeOn += CorrectHighCLTimeOnEffect * (ScaleEffectsWithLives ? m_remaningLives : 1);
                }
                else if(clMean > 0.35){ // Correct Round with close to optimal CL 
                    m_timeOn += CorrectMedCLTimeOnEffect * (ScaleEffectsWithLives ? m_remaningLives : 1);
                }
                else{ // Correct Round with Low CL 
                    m_timeOn += CorrectLowCLTimeOnEffect * (ScaleEffectsWithLives ? m_remaningLives : 1);
                }
            }
            // Give the user more time to memorize
            else{
                if(clMean > 0.75){ // Having a real hard time
                    m_timeOn += FailHighCLTimeOnEffect * (ScaleEffectsWithLives ? (Lives - m_remaningLives) : 1);
                }
                else if(clMean > 0.35){ // May need some help
                    m_timeOn += FailMedCLTimeOnEffect * (ScaleEffectsWithLives ? (Lives - m_remaningLives) : 1);
                }
                else { // Low CL - try again
                    m_timeOn += FailLowCLTimeOnEffect * (ScaleEffectsWithLives ? (Lives - m_remaningLives) : 1);
                }
            }

            //Minimum time on
            m_timeOn = Mathf.Max(m_timeOn, 0.5f);
            m_timeOn = Mathf.Max(m_timeOn, 0.1f);
        }

        protected virtual void BlockInteraction(){
            foreach(FollowButton button in Buttons){
                button.SetInteractable(false);
            }
        }

        protected virtual void AllowInteraction(){
            foreach(FollowButton button in Buttons){
                button.SetInteractable(true, RecordClick);
            }
        }

        //Allow unity testing without headset
        private void Update() {
            if(Input.GetKeyDown("s")){
                StartGame();
            }
        }
    }
}
using System;
using System.Collections;
using Architecture.Data;
using Architecture.SettingsArea;
using HomeTools;
using Theming;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainActivity
{
    // Component for logo scene
    public class LogoLoad : MonoBehaviour
    {
        public string scene; // Name of main scene

        private void Start()
        {
            SceneDataTransfer.CurrentData = DateTime.Now; // Save current time
            AppData.Initialize(); // Initialize app data
            var newLoad = Load(); // Get load enumerator
            StartCoroutine(newLoad); // Start enumerator
            Application.targetFrameRate = 60; // Setup target frame rate of app
        }

        // Load main scene
        private IEnumerator Load()
        {
            // Create async operation
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;

            // Setup app status bar
            ApplicationChrome.statusBarState = ApplicationChrome.States.TranslucentOverContent;
            ApplicationChrome.navigationBarState = ApplicationChrome.States.Visible;
            
            // Setup color of status bar
            AppTheming.SetUpNavigationBar();
            AppTheming.UpdateOtherElements();
            AppParameters.LoadedFromLogo = true;
        
            // Wait for load
            yield return new WaitForSeconds(0.57f);
            while (asyncLoad.isDone)
                yield return null;

            asyncLoad.allowSceneActivation = true;
            yield return new WaitForSeconds(Time.deltaTime * 2);
            // Create main systems
            CreateEntryPoint();
            yield return new WaitForSeconds(Time.deltaTime * 2);
            // Activate main scene
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
            // Colorize main page
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.WorkArea);
            // Activate logo scene
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
            yield return new WaitForSeconds(Time.deltaTime * 2);
            // Unload logo scene
            SceneManager.UnloadSceneAsync(0);
            // Enable Graphic Raycaster
            MainCanvas.RectTransform.GetComponent<GraphicRaycaster>().enabled = true;
        }

        // Create main systems
        private void CreateEntryPoint()
        {
            AppParameters.LoadedFromLogo = false;
            LanguageSettings.TryInitialize();
            MainCanvas.RectTransform.gameObject.AddComponent<EntryPointSeaTodo>();
            AppParameters.LoadedFromLogo = true;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Thirdweb.Examples
{


    public class Prefab_ValidatorLoader : MonoBehaviour
    {
        [Header("SETTINGS")]
        public string validatorAddress = "https://lcd.serv.services/cosmos/staking/v1beta1/validators";

        [Header("UI ELEMENTS (DO NOT EDIT)")]
        public Transform contentParent;
        public Prefab_Validator validatorPrefab;
        public GameObject loadingPanel;

        private void Start()
        {
            foreach (Transform child in contentParent)
                Destroy(child.gameObject);

            StartCoroutine(LoadValidators(validatorAddress));
        }

        IEnumerator LoadValidators(string url)
        {
            loadingPanel.SetActive(true);

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = webRequest.downloadHandler.text;

                    // Deserialize JSON response into array of Validator struct
                    ValidatorList validators = JsonUtility.FromJson<ValidatorList>(jsonResponse);
                    Debug.Log("Json: " + jsonResponse );

                    // Instantiate Prefab_Validator for each Validator in the array
                    foreach (Validator validator in validators.validators)
                    {
                        Prefab_Validator nftPrefabScript = Instantiate(validatorPrefab, contentParent);
                        nftPrefabScript.LoadValidator(validator);
                    }
                }
                else
                {
                    Debug.LogError("Error: " + webRequest.error);
                }
            }

            loadingPanel.SetActive(false);
        }
    }

    // Helper class to deserialize JSON array into array of structs
    [System.Serializable]
    public class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.items;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] items;
        }
    }

}

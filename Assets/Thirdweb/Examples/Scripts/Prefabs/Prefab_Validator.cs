using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

namespace Thirdweb.Examples
{
    public class Prefab_Validator : MonoBehaviour
    {
        [Header("UI ELEMENTS")]
        public Button validatorButton;

        public TMP_Text validatorName;
        public Image validatorImage;
        public TMP_Text validatorTotalStake;
        public TMP_Text validatorRate;
        public TMP_Text validatorFees;

        public async void LoadValidator(Validator validator)
        {
            validatorName.text = validator.description.moniker;
            validatorTotalStake.text = validator.delegator_shares;
           // validatorImage.sprite = await ThirdwebManager.Instance.SDK.storage.DownloadImage(validator.metadata.image);
            validatorButton.onClick.RemoveAllListeners();
            validatorButton.onClick.AddListener(() => DoSomething(validator));
        }

        void DoSomething(Validator validator)
        {
            Debugger.Instance.Log(validator.description.moniker, validator.ToString());
        }
    }
}

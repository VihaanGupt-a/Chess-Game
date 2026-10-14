using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RoomCodeAutoFocus : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] inputFields = new TMP_InputField[6];
    private UnityAction<string>[] callbacks;

    private void Awake()
    {
        callbacks = new UnityAction<string>[inputFields.Length];

        for (int i = 0; i < inputFields.Length; i++)
        {
            int index = i;

            callbacks[i] = value => OnCharacterEntered(index, value);

            if (inputFields[i] != null)
                inputFields[i].onValueChanged.AddListener(
                    callbacks[i]
                );
        }
    }

    private void OnDestroy()
    {
        if (callbacks == null)
            return;

        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i] != null && callbacks[i] != null)
                inputFields[i].onValueChanged.RemoveListener(callbacks[i]);
        }
    }

    private void OnCharacterEntered(int index, string value)
    {
        if (inputFields[index] == null)
            return;

        if (string.IsNullOrEmpty(value))
            return;

        if (value.Length > 1)
        {
            value = value.Substring(0, 1);
            inputFields[index].SetTextWithoutNotify(value);
        }

        if (index < inputFields.Length - 1 && inputFields[index + 1] != null)
        {
            inputFields[index + 1].Select();
            inputFields[index + 1].ActivateInputField();
        }
    }
}






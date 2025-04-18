using UnityEngine;

using UnityEngine.Networking;

using System.Text;

using System.Collections;

using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class AvatarSender : MonoBehaviour

{

    [Header("UI Elements")]

    public GameObject[] avatars;

    public Button confirmButton;

    private int selectedAvatarNumber = -1;

    void Start()

    {

        confirmButton.gameObject.SetActive(false);

    }

    public void SelectAvatar(int avatarNumber)

    {

        selectedAvatarNumber = avatarNumber;

        for (int i = 0; i < avatars.Length; i++)

        {

            Image avatarImage = avatars[i].GetComponent<Image>();

            if (avatarImage == null)

            {

                Debug.LogError($"Avatar {i} does not have an Image component.");

                continue;

            }

            Color avatarColor = avatarImage.color;

            if (i == avatarNumber)

            {

                avatarColor.a = 1f;

            }

            else

            {

                avatarColor.a = 0.5f;

            }

            avatarImage.color = avatarColor;

        }

        confirmButton.gameObject.SetActive(true);

    }

    [System.Serializable]

    public class UserAvatar

    {

        public int head;

        public int body;

        public int legs;

    }

    public void SendAvatarWithNetworkManager(int avatarNumber)

    {

        UserAvatar avatar = new UserAvatar

        {

            head = avatarNumber,

            body = avatarNumber,

            legs = avatarNumber

        };


        NetworkManager.Instance.MakeRequest<UserAvatar>(

            "avatar/set",

            RequestType.POST,

            avatar,

            response =>

            {

                if (response != null)

                {

                    Debug.Log($"✅ Avatar {avatarNumber} sent successfully!");

                    UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");

                }

                else

                {

                    Debug.LogError($"❌ Failed to send avatar {avatarNumber}");

                }

            }

        );

    }

    public void ConfirmSelection()

    {

        if (selectedAvatarNumber != -1)

        {

            SendAvatarWithNetworkManager(selectedAvatarNumber);

        }

    }

}

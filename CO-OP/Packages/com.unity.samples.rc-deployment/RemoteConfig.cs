// NOTE: You need to deploy the new-remote-config.rc file before being able to run this sample,
// to do so, open the deployment window and deploy the new-remote-config.rc file.
// See README.md for more details.

using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.RemoteConfig;
using UnityEngine.UIElements;

namespace Sample.Deployment.RemoteConfigSample
{
    public class RemoteConfig : MonoBehaviour
    {
        [SerializeField]
        UIDocument m_UIDocument;

        Label m_StringValue;
        Label m_IntValue;
        Label m_BoolValue;
        Label m_LongValue;
        Label m_FloatValue;
        Label m_JsonValue;
        Button m_ButtonGetConfig;

        public Action<RuntimeConfig> OnRemoteConfigUpdated;
        struct userAttributes
        {
            // Optionally declare variables for any custom user attributes:
            public bool expansionFlag;
        }

        struct appAttributes
        {
            // Optionally declare variables for any custom app attributes:
            public int level;
            public int score;
            public string appVersion;
        }

        async void Awake()
        {
            InitializeUi();

            // Remote Config needs to be initialized and then the user must sign in.
            await InitializeServices();
            await SignInAnonymously();
            await FetchConfigsAsync();
        }

        void InitializeUi()
        {
            m_StringValue = m_UIDocument.rootVisualElement.Q<Label>("StringValue");
            m_IntValue = m_UIDocument.rootVisualElement.Q<Label>("IntValue");
            m_BoolValue = m_UIDocument.rootVisualElement.Q<Label>("BoolValue");
            m_LongValue = m_UIDocument.rootVisualElement.Q<Label>("LongValue");
            m_FloatValue = m_UIDocument.rootVisualElement.Q<Label>("FloatValue");
            m_JsonValue = m_UIDocument.rootVisualElement.Q<Label>("JsonValue");
            m_ButtonGetConfig = m_UIDocument.rootVisualElement.Q<Button>("ButtonGetConfig");
            m_ButtonGetConfig.clicked += async () => await FetchConfigsAsync();
        }

        async Task FetchConfigsAsync()
        {
            m_ButtonGetConfig.SetEnabled(false);
            var config = await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
            UpdateValues(config);
            m_ButtonGetConfig.SetEnabled(true);
        }

        static async Task InitializeServices()
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
            }
        }

        static async Task SignInAnonymously()
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        void UpdateValues(RuntimeConfig config)
        {
            m_StringValue.text = config.GetString("string_key");
            m_IntValue.text = config.GetInt("int_key").ToString();
            m_BoolValue.text = config.GetBool("bool_key") ? "true" : "false";
            m_LongValue.text = config.GetLong("long_key").ToString();
            m_FloatValue.text = config.GetFloat("float_key").ToString(CultureInfo.InvariantCulture);
            m_JsonValue.text = config.GetJson("json_key");
        }
    }
}

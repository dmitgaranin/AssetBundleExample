using System.Reflection;
using UnityEngine;

namespace Assets.Scripts
{
    public class BundleScene : MonoBehaviour
    {
        public const string SceneName = "Scene";
        public const string PlayerName = "Player";
        public const string MoveClassName = "Move";
        public const string MoveMethodName = "GetNextPosition";
        public const float Speed = 0.9f;

        public GameObject Scene;
        public GameObject Player;

        private AssetBundle _bundle;
        private bool _isMovement;
        private MethodInfo _moveMethod;
        private object _moveClass;

        public void SetBundle(AssetBundle bundle)
        {
            _bundle = bundle;
        }

        public void Load()
        {
            Scene = (GameObject)Instantiate(_bundle.LoadAsset(SceneName));
            Player = (GameObject)Instantiate(_bundle.LoadAsset(PlayerName));
            LoadScriptMovePlayer();
        }

        private void LoadScriptMovePlayer()
        {
            var scriptContent = _bundle.LoadAsset(MoveClassName) as TextAsset;
            if (scriptContent == null)
            {
                Debug.Log("ScriptContent was NULL");
                return;
            }
            var assembly = Assembly.Load(scriptContent.bytes);
            var type = assembly.GetType(MoveClassName);
            _moveClass = assembly.CreateInstance(MoveClassName);
            _moveMethod = type.GetMethod(MoveMethodName);
        }

        public void Update()
        {
            if (_isMovement && Player != null)
            {
                var nextPos = (Vector3)_moveMethod.Invoke(
                    _moveClass, 
                    new object[]
                    {
                        Player.transform.position,
                        Scene.GetComponent<Renderer>().bounds.size
                    }); 
                var step = Speed * Time.deltaTime;
                Player.transform.position = Vector3.MoveTowards(Player.transform.position, nextPos, step);
            }
        }
    
        public void StartMovePlayer()
        {
            _isMovement = true;
        }

        public void StopMovePlayer()
        {
            _isMovement = false;
        }

        public void Clear()
        {
            Destroy(Scene);
            Destroy(Player);
        }
    }
}

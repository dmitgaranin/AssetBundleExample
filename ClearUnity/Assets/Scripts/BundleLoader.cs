using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class BundleLoader : MonoBehaviour
    {
        private string _bundleUrl;
        private AssetBundle _bundle;
        private BundleScene _scene;

        public void Start()
        {
            var creatorButtons = gameObject.GetComponent<CreatorButtonsMenu>();
            creatorButtons.CreateButtonsMenuToLoadScenes(LoadNewScene);
        }

        private IEnumerator Work()
        {
            if (_bundleUrl == null)
            {
                Debug.Log("Trouble with bundleUrl");
                yield break;
            }
            WWW www = new WWW(_bundleUrl);
            yield return www;
            if (www.error != null)
                throw new Exception("Was error:" + www.error);
            if (!www.isDone)
                yield break;
            if (_bundle != null)
                DeleteCurrentScene();
            _bundle = www.assetBundle;
            SpawnScene();
        }

        public void DeleteCurrentScene()
        {
            if (_bundle != null)
                _bundle.Unload(true);
            if (_scene != null)
            {
                _scene.Clear();
                Destroy(_scene);
            }
        }

        public void LoadNewScene(string nameScene)
        {
            _bundleUrl = UriConstructor.GetUriFileLocationPath(nameScene);
            StartCoroutine(Work());
        }

        public void SpawnScene()
        {
            Destroy(gameObject.GetComponent<BundleScene>());
            _scene = gameObject.AddComponent<BundleScene>();
            _scene.SetBundle(_bundle);
            _scene.Load();
            _scene.StartMovePlayer();
        }
    }
}

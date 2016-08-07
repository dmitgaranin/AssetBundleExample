# AssetBundleExample
Демонстрация работы с Asset Bundle в Unity3D

В этом проекте:
* __AssemblyVS__ - сборка исходного кода в _library_-файлы
* __ClearUnity__ - проект в Unity, демонстрирующий мощь _assetBundle_'ов (без прочих зависимостей вида префабов или скриптов)
* __ProjectUnity__ - основной проект, в котором создаются _assetBundle_


# Этапы создания Asset Bundle

## 1. Сборка Assembly

Есть два варианта:
1. Сборка через Visual Studio/Mono/etc (_использовано в этом проекте_);
2. Сборка через консоль

### Visual Studio
1. Создать новый проект `Class Library` (можно использовать текущий - `Move`);
2. Реализовать необходимый код;
3. Правой кнопкой на `Solution` -> `Build Solution`;
4. Любимым файлововым менеджером открываем этот проект;
5. Переходим в папку `Debug` (`<путь к проекту>` -> `bin` -> `Debug`);
6. У собранного _library_-файла меняем расширение на `.bytes` (т.к. в assetBunlde нельзя положить `.dll`, но двочиные данные можно);
7. __Profit__ Теперь этот файл можно добавлять в bundle

### Консоль
Процесс ничем не отличается от обычной сборки через консоль:
1. Открываем консоль;
2. Указываем путь до компилятора `csc.exe` (обычно лежит в `../Windows/Microsoft.NET/Framework/<версия>/`);
3. Указываем пути до зависимых библиотек (например, если используется `UnityEngine.dll`, пишем `/r:<путь до Unity3D>/Editor/Data/Managed/UnityEngine.dll`, где `/r` - ключ _reference_, подробности в документации);
4. Указываем, во что собирать. В данном случае `/target:library`;
5. И путь до файла(ов), которые хотим собрать;
6. Также используйте ключ `-nologo`, чтобы скрыть отображение логотипа и предупреждений.
7. __Profit__ Теперь этот файл можно добавлять в bundle

## 2. Сборка Asset Bundle
О том, как указать в редакторе, какие объекты следует поместить в Asset Bundle читайте в [документации](http://docs.unity3d.com/Manual/BuildingAssetBundles.html)

---

В `ProjectUnity` из префабов собраны две сцены. В каждой из них используются одноименные файлы:
* префаб `Scene` - площадка, на которой перемещается Player;
* префаб `Player` - объект, который перемещается в соответствии с правилами, описанными в скрпите;
* а также скрипт `Move.bytes` (бывший `Move.dll`, см. пункт №1), который в зависимости от положения Player'а решает, куда переместить его дальше.

Название файлов важно, так как только по ним можно будет достать сами объекты из Asset Bundle. Поэтому, для каждой сцены используются одноименные файлы, но с разными тегами (`scene1` для всех объектов первой сцены и `scene2` - для второй).

---

Когда все файлы для Asset Bundle отмечены, нужно их собрать: в меню редактора `Assets -> Build AssetBundles` (начинает работать скрипт `BuildBundler.cs`). Все собранные bundle расположены в `ProjectUnity/Assets/AssetBundles`. Ключевыми являются файлы без раширения (или с расширением, так как _в качестве тега-названия для Asset Bundle могут быть названия, содержащие точки_).

## 3. Работаем с Asset Bundle

__ВАЖНО:__ чтобы работать с локальными Asset Bundle, необходимо указывать до них путь в виде:

```
file://localhost/<drive>|/<path>
file:///<drive>|/<path>
file://localhost/<drive>:/<path>
file:///<drive>:/<path>
```
[ссылка на wiki](https://en.wikipedia.org/wiki/File_URI_scheme)

---

О загрузке и работе с bundle можно найти [здесь](http://docs.unity3d.com/Manual/DownloadingAssetBundles.html)

Но этого достаточно только для работы с префабами, чтобы использовать вложенные файлы с кодом, нужно просто использовать рефлексию, как будто бы и нет никакого bundle.

Вот пример загрузки класса, с помощью которого будем передвигать `Player`

```c#
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
```

И чтобы вызвать метод этого класса поможет метод `Invoke`:
[ссылка](https://msdn.microsoft.com/ru-ru/library/a89hcwhh.aspx)


## Небольшие заметки

WebGL и Asset Bundle дружат очень хорошо. Но для этого, при сборке необходимо об этом указать третьим параметром в `BuildBundler.cs`

```c#
[MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(
            UnityConstants.PathToAssetBundles,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows);
    }
```

Так, для Windows это будет `BuildTarget.StandaloneWindows`, для WebGL - `BuildTarget.WebGL`

---

__НО__ файлы `.bytes` не будут работать (никакого исполняемого кода в вебе), так как
> No, there is no way around this restriction with reflection.
The key difference between the web player and WebGL build targets in Unity in this case is that WebGL uses AOT (ahead-of-time) compilation, whereas the web player uses JIT (just-in-time) compilation. With AOT compilation, it is not possible to load an assembly at run-time that was not present at compile time.
Of course, it is possible to load JavaScript code at runtime, so as you suggest, you'll probably need to go this route.

[источник](http://answers.unity3d.com/questions/971373/how-to-load-an-assetbundle-with-scripts-in-webgl.html)

:(

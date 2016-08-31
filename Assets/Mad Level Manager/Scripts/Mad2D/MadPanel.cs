/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using System.Reflection;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_4_6 || UNITY_5 || UNITY_6 || UNITY_7 // should work for the next 6 years or so :-) TODO: Remove later
using UnityEngine.EventSystems;
#endif

using MadLevelManager;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(MadBigMeshRenderer))]
[RequireComponent(typeof(MadMaterialStore))]
public class MadPanel : MadNode {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    private static List<MadPanel> panels = new List<MadPanel>();

    public RenderMode renderMode = RenderMode.Legacy;

    public bool halfPixelOffset = true;

    public bool hideInvisibleSprites = false;

        public GameObject animationFeedbackPrefab;

    // if set to true, all input is ignored
    [NonSerialized]
    public bool ignoreInput = false;

    [NonSerialized]
    public HashSet<MadSprite> sprites = new HashSet<MadSprite>();

    public MadMaterialStore materialStore {
        get {
            if (_materialStore == null) {
                _materialStore = gameObject.AddComponent<MadMaterialStore>();
            }

            return _materialStore;
        }

        private set {
            _materialStore = value;
        }
    }
    private MadMaterialStore _materialStore;

    [HideInInspector]
    MadSprite _focusedSprite;
    int _focusedSpriteModCount;
    public MadSprite focusedSprite {
        get { return _focusedSprite; }
        set {
            _focusedSprite = value;
            _focusedSpriteModCount++;
            if (onFocusChanged != null) {
                onFocusChanged(_focusedSprite);
            }
        }
    }

    public Camera currentCamera {
        get {
            if (_currentCamera == null || (_currentCamera.cullingMask & (1 << gameObject.layer)) == 0) {
                _currentCamera = null;

                Camera[] cameras = FindObjectsOfType(typeof(Camera)) as Camera[];

                for (int i = 0; i < cameras.Length; ++i) {
                    var camera = cameras[i];
                    if ((camera.cullingMask & (1 << gameObject.layer)) != 0) {
                        if (_currentCamera != null) {
                            Debug.Log("There are multiple cameras that are rendering the \""
                                + LayerMask.LayerToName(gameObject.layer)
                                + "\" layer. Please adjust your culling masks and/or change layer of this Panel object.", this);
                        } else {
                            _currentCamera = camera;
                        }
                    }
                }
            }

            return _currentCamera;
        }
    }

    private Camera _currentCamera;

#if UNITY_4_6 || UNITY_5 || UNITY_6 || UNITY_7 // should work for the next 6 years or so :-) TODO: Remove later
    public List<GameObject> unityUiIgnore = new List<GameObject>();

    private StandaloneInputModule uGUIStandaloneInputModule;
    private MethodInfo uGUIGetLastPointerMethod;
    private List<RaycastResult> uGUIRaycastResult;
    private object[] uGUIGetLastPointerMethodArgs;
#endif

    // input helpers
    HashSet<MadSprite> hoverSprites = new HashSet<MadSprite>();

    // helper for making Unity Remote working
    bool haveTouch;

    // set of sprites that has been clicked or touched. When mouse button or finger is up,
    // and sprite still resists in here, it may be treated as "pressed"
    HashSet<MadSprite> touchDownSprites = new HashSet<MadSprite>();
    private static HashSet<MadSprite> EmptyMadSpriteHashSet = new HashSet<MadSprite>();
    private static List<MadSprite> EmptyMadSpriteList = new List<MadSprite>();

    // ===========================================================
    // Events
    // ===========================================================

    public delegate void Event1<T>(T t);

    /// <summary>
    /// Occurs when on focus changed. Passes focused sprite or null if nothing is currently focused.
    /// </summary>
    public event Event1<MadSprite> onFocusChanged;

    // ===========================================================
    // Methods
    // ===========================================================

    // trial version methods
    void OnGUI() {
        if (MadTrial.isTrialVersion) {
            MadTrial.InfoLabel("This is an evaluation version of Mad Level Manager");
        }
    }

    public Vector3 WorldToPanel(Camera worldCamera, Vector3 worldPos) {
        var pos = worldCamera.WorldToScreenPoint(worldPos);
        pos = currentCamera.ScreenToWorldPoint(pos);
        pos.z = 0;
        return pos;
    }

    void OnEnable() {
        Unity5Check();

        panels.Add(this);

        materialStore = GetComponent<MadMaterialStore>();

        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null) {
            MadGameObject.SafeDestroy(meshRenderer);
        }

        var meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null) {
            MadGameObject.SafeDestroy(meshFilter);
        }

    }

    void Start() {
        TryInitializeUnityUI();
    }

    private void TryInitializeUnityUI() {
#if UNITY_4_6 || UNITY_5 || UNITY_6 || UNITY_7 // should work for the next 6 years or so :-) TODO: Remove later
        if (Application.isPlaying && EventSystem.current != null) {
            uGUIStandaloneInputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
            var type = uGUIStandaloneInputModule.GetType();
            uGUIGetLastPointerMethod = type.GetMethod("GetLastPointerEventData",
                BindingFlags.NonPublic | BindingFlags.Instance);
            uGUIRaycastResult = new List<RaycastResult>();
            uGUIGetLastPointerMethodArgs = new object[] { -1 };
        }
#endif
    }

#pragma warning disable 429, 162
    private void Unity5Check() {

        // generated check
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if ("MadLevelManager" != ("_NAME" + "SPACE_") && "MadLevelManager" != "MadLevelManager") {
            return;
        }

        if (Application.isPlaying) {
            return;
        }

#if UNITY_EDITOR
        /*int majorVersion;
        if (int.TryParse(Application.unityVersion.Split('.')[0], out majorVersion)) {
            if (majorVersion >= 5 && renderMode == RenderMode.Legacy) {
                var nonAtlasSprite = MadTransform.FindChild<MadSprite>(transform,
                    (s) => {
                        if (s is MadText) {
                            return ((MadText) s).atlas == null;
                        } else {
                            return s.inputType == MadSprite.InputType.SingleTexture && MadGameObject.IsActive(s.gameObject);
                        }
                    });
                if (nonAtlasSprite != null) {
                    bool sw = EditorUtility.DisplayDialog(
                        "Unity 5 Compatibility",
                        "With Unity 5 is recommended to switch to Depth-Based render mode or Legacy mode with atlases only. " +
                        "Please see Unity 5 Compatibility docmentation page for more information. " +
                        "Do you want to switch to Depth-Based mode?\n\n",
                        "Yes (Recommended)", "No");
                    if (sw) {
                        renderMode = RenderMode.DepthBased;
                    }
                }
            }
        }*/
#endif
    }
#pragma warning restore 429, 162

    void OnDisable() {
        panels.Remove(this);
    }

    void Update() {
        // fix the offset
        if (halfPixelOffset) {
            var root = FindParent<MadRootNode>();
            float pixelSize = root.pixelSize;

            float x = 0, y = 0;
            if (Screen.height % 2 == 0) {
                y = pixelSize;
            }

            if (Screen.width % 2 == 0) {
                x = pixelSize;
            }

            MadTransform.SetLocalPosition(transform, new Vector3(x, y, 0));
        }

        UpdateInput();
    }

    void UpdateInput() {
        if (ignoreInput) {
            return;
        }

#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY
        UpdateTouchInput();
        if (Application.isEditor) {
            UpdateMouseInput();
        }
#else
        UpdateMouseInput();
#endif
    }

    void UpdateTouchInput() {
        var touches = Input.touches;
        var sprites = new HashSet<MadSprite>();

        bool ignorePointer = false;

        CheckIgnorePointer(ref ignorePointer);

        // to make Unity Remote work
        haveTouch = Input.touchCount > 0;

        if (touches.Length == 1) {
            var touch = touches[0];

                if (touch.phase == TouchPhase.Ended)
                {
                    ActivateFeedbackAnimation(touch.position);
                }

                List<MadSprite> allSprites;
            if (!ignorePointer) {
                allSprites = AllSpritesForScreenPoint(touch.position);
            } else {
                allSprites = EmptyMadSpriteList;
            }

            for (int i = 0; i < allSprites.Count; ++i) {
                sprites.Add(allSprites[i]);
            }

            foreach (var sprite in sprites) {

                if (touch.phase == TouchPhase.Began) {
                    touchDownSprites.Add(sprite);

                    sprite.onTouchEnter(sprite);

                } else if (touch.phase == TouchPhase.Ended && touchDownSprites.Contains(sprite)) {
                    sprite.onTap(sprite);
                    sprite.TryFocus();
                } else {
                    // will remove sprite from mouse down if dragging is registered
                    if (IsDragging(sprite)) {
                        sprite.onTouchExit(sprite);
                        touchDownSprites.Remove(sprite);
                    }
                }
            }
        }

        // find sprites that are no longer hovered
        //if (sprites.Count != touchDownSprites.Count) {
        List<MadSprite> unhovered = new List<MadSprite>();
        foreach (var touchDownSprite in touchDownSprites) {
            if (!sprites.Contains(touchDownSprite)) {
                unhovered.Add(touchDownSprite);
                touchDownSprite.onTouchExit(touchDownSprite);
            }
        }

        foreach (var u in unhovered) {
            hoverSprites.Remove(u);
        }
        //}
    }

        void OnDrawGizmos()
        {
            Camera camera = Camera.main;
            Vector3 p = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -0.5F));
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(p, 0.2F);
        }

        private void ActivateFeedbackAnimation(Vector3 screenPos)
    {
            Camera camera = Camera.main;
            Vector3 p = camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, camera.nearClipPlane));
            GameObject.Instantiate(animationFeedbackPrefab, p, Quaternion.identity);
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawSphere(p, 0.1F);
        }

    void UpdateMouseInput() {
        if (haveTouch) {
            // if I am here this means that Unity Remote is working
            return;
        }

        bool ignorePointer = false;

        CheckIgnorePointer(ref ignorePointer);

        HashSet<MadSprite> screenPointSprites;

        if (!ignorePointer) {
            screenPointSprites = new HashSet<MadSprite>(AllSpritesForScreenPoint(Input.mousePosition));
        } else {
            screenPointSprites = EmptyMadSpriteHashSet;
        }

        foreach (var sprite in screenPointSprites) {
            // find newly hovered sprites
            if (hoverSprites.Add(sprite)) {
                sprite.onMouseEnter(sprite);
            }

            // check if any of the sprites have registered draggable which is in dragging state
            // if so, it should be removed from mouseDownSprites
            if (IsDragging(sprite)) {
                touchDownSprites.Remove(sprite);
            }
        }

        // find sprites that are no longer hovered
        if (screenPointSprites.Count != hoverSprites.Count) {
            List<MadSprite> unhovered = new List<MadSprite>();
            foreach (var hoverSprite in hoverSprites) {
                if (!screenPointSprites.Contains(hoverSprite)) {
                    unhovered.Add(hoverSprite);
                    hoverSprite.onMouseExit(hoverSprite);
                }
            }

            foreach (var u in unhovered) {
                hoverSprites.Remove(u);
            }
        }

        if (!ignorePointer && Input.GetMouseButtonDown(0)) {
                ActivateFeedbackAnimation(Input.mousePosition);
            foreach (var sprite in hoverSprites) {
                sprite.onMouseDown(sprite);
                touchDownSprites.Add(sprite);
            }
        }

        if (!ignorePointer && Input.GetMouseButtonUp(0)) {
            int modCount = _focusedSpriteModCount;
            foreach (var mouseDownSprite in touchDownSprites) {
                if (screenPointSprites.Contains(mouseDownSprite)) {
                    mouseDownSprite.onMouseUp(mouseDownSprite);
                    mouseDownSprite.TryFocus();
                }
            }

            touchDownSprites.Clear();

            if (modCount == _focusedSpriteModCount && focusedSprite != null) {
                // focus lost to nothing
                focusedSprite.hasFocus = false;
            }
        }
    }

    private void CheckIgnorePointer(ref bool ignorePointer) {
#if UNITY_4_6 || UNITY_5 || UNITY_6 || UNITY_7 // should work for the next 6 years or so :-) TODO: Remove later
        if (EventSystem.current != null) {
            if (uGUIGetLastPointerMethod == null) {
                TryInitializeUnityUI();
            }

            if (uGUIGetLastPointerMethod == null) {
                return;
            }

            PointerEventData ped =
                (PointerEventData) uGUIGetLastPointerMethod.Invoke(uGUIStandaloneInputModule, uGUIGetLastPointerMethodArgs);
            if (ped != null) {
                EventSystem.current.RaycastAll(ped, uGUIRaycastResult);

                for (int j = 0; j < uGUIRaycastResult.Count; ++j) {
                    var go = uGUIRaycastResult[j].gameObject;

                    if (unityUiIgnore.Count == 0) {
                        // no ignore list and something hit
                        ignorePointer = true;
                    }

                    for (int i = 0; i < unityUiIgnore.Count; i++) {
                        var ignore = unityUiIgnore[i];
                        if (go.transform != ignore.transform && !go.transform.IsChildOf(ignore.transform)) {
                            // hit sprite is not on ignore list
                            ignorePointer = true;
                            break;
                        }
                    }
                }
            }
        }
#endif
    }

    bool IsDragging(MadSprite sprite) {
        var dragHandler = sprite.FindParent<MadDraggable>();
        if (dragHandler != null) {
            if (dragHandler.dragging) {
                return true;
            }
        }

        return false;
    }

    List<MadSprite> AllSpritesForScreenPoint(Vector2 point) {
        List<MadSprite> result = new List<MadSprite>();

        var ray = currentCamera.ScreenPointToRay(point);
        RaycastHit[] hits = Physics.RaycastAll(ray, 4000);
        foreach (var hit in hits) {
            var collider = hit.collider;
            var sprite = collider.GetComponent<MadSprite>();
            if (sprite != null && sprite.panel == this) {
                result.Add(sprite);
            }
        }

        return result;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    public static MadPanel FirstOrNull(Transform currentTransform) {
        // first try to find panel as parent
        if (currentTransform != null) {
            var panel = MadTransform.FindParent<MadPanel>(currentTransform);
            if (panel != null) {
                return panel;
            }
        }

        // then try to locate the panel on the static list
        if (panels.Count > 0) {
            return panels[0];
        }

        // if all above fails, try to locate panel using the slow FindObjectOfType method
        return GameObject.FindObjectOfType(typeof(MadPanel)) as MadPanel;
    }

    public static MadPanel UniqueOrNull() {
        if (MadPanel.panels.Count == 1) {
            return MadPanel.panels[0];
        }

        var panels = GameObject.FindObjectsOfType(typeof(MadPanel));
        if (panels.Length == 1) {
            return panels[0] as MadPanel;
        } else {
            return null;
        }
    }

    public static MadPanel[] All() {
        return panels.ToArray();
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

    public enum RenderMode {
        Legacy,
        DepthBased,
    }

}

#if !UNITY_3_5
} // namespace
#endif
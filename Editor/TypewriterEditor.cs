using Aarthificial.Typewriter.Editor.Extensions;
using Aarthificial.Typewriter.Editor.Layout;
using Aarthificial.Typewriter.Editor.Layout.Inspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Typewriter.Editor {
  public class TypewriterEditor : EditorWindow {
    private DatabaseView _database;
    private InspectorView _inspector;
    private bool _isPortrait;

    private VisualElement _main;
    private TabbedView _tabs;
    private Toolbar _toolbar;
    public static TypewriterEditor Instance { get; private set; }

    private void OnEnable() {
      Instance = this;
      titleContent = new GUIContent {
        text = "Typewriter",
        image = Resources.Load<Texture2D>("Textures/TypewriterWindow"),
      };
    }

    private void OnDisable() {
      Instance = null;
    }

    public void CreateGUI() {
      var visualTree = Resources.Load<VisualTreeAsset>("UXML/TypewriterEditor");
      var styleSheet = Resources.Load<StyleSheet>("Styles/TypewriterEditor");

      visualTree.CloneTree(rootVisualElement);
      rootVisualElement.styleSheets.Add(styleSheet);
      _database = rootVisualElement.Q<DatabaseView>();
      _inspector = rootVisualElement.Q<InspectorView>();
      _toolbar = rootVisualElement.Q<Toolbar>();
      _main = rootVisualElement.Q<VisualElement>("main");
      _main.RegisterCallback<GeometryChangedEvent>(HandleGeometryChanged);
      _database.SelectionChanged += Refresh;
      HandleGeometryChanged(null);
      Refresh();
    }

    [MenuItem("Tools/Typewriter")]
    public static void Open() {
      GetWindow<TypewriterEditor>();
    }

    private void HandleGeometryChanged(GeometryChangedEvent _) {
      var isPortrait = _main.layout.width < _main.layout.height;
      if (_isPortrait != isPortrait) {
        _isPortrait = isPortrait;
        _main.EnableInClassList("portrait", isPortrait);
      }
    }

    public int GetSelectedEntry() {
      return _database.EntryProperty?.FindEntryID() ?? 0;
    }

    private void Refresh() {
      _toolbar.BindCategory(_database.TableProperty);
      _toolbar.BindEntry(_database.EntryProperty);
      _inspector.BindProperty(_database.EntryProperty);
    }
  }
}

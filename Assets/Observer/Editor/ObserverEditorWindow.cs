using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObserverEditorWindow : EditorWindow {

    private string observerQuery = "";

    private static readonly Regex searchString = new Regex(@"(?<gameObjectName>.+)\.(?<componentName>.+)\.(?<memberName>.+)");

    [MenuItem("Tools/Observer %`")]
    public static void CreateMyThingymabob()
    {
        GetWindow<ObserverEditorWindow>(false, "Observer", true);
    }

    public void OnGUI()
    {
        observerQuery = EditorGUILayout.TextField(observerQuery);

        var matchedExpression = searchString.Match(observerQuery);

        var observedName = matchedExpression.Groups["gameObjectName"].Value;
        var observedComponent = matchedExpression.Groups["componentName"].Value;
        var observedMember = matchedExpression.Groups["memberName"].Value;
        if (!string.IsNullOrEmpty(observedName) &&
            !string.IsNullOrEmpty(observedComponent) &&
            !string.IsNullOrEmpty(observedMember))
        {
            var gameObjects = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(gobj => gobj.scene.isLoaded)
                .Where(gobj => gobj.name.Contains(observedName));
            foreach (var gobj in gameObjects)
            {
                var component = FindComponentOfGameObject(gobj, observedComponent);
                if (component != null)
                {
                    var info = GetMemberByName(component, observedMember);
                    if (info != null)
                    {
                        var type = GetMemberType(info);
                        if (type == typeof(Vector3))
                        {
                            SetValueOfMember<Vector3>(info, EditorGUILayout.Vector3Field(gobj.name, GetValueOfMember<Vector3>(info, component)), component);
                        }
                    }
                }
                else
                {
                    EditorGUILayout.LabelField(String.Format("The component \"{0}\" can't be found on this Game Object", observedComponent));
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please provide the name of an object, the component to observe and the field/property to watch", MessageType.Warning);
        }
    }

    private Component FindComponentOfGameObject(GameObject gobj, string observedComponent)
    {
        var components = gobj.GetComponents<Component>();
        return components.FirstOrDefault(component => component.GetType().Name.Equals(observedComponent, StringComparison.InvariantCultureIgnoreCase));
    }

    private Type GetMemberType(MemberInfo info)
    {
        switch (info.MemberType)
        {
            case MemberTypes.Field:
                return ((FieldInfo)info).FieldType;
            case MemberTypes.Property:
                return ((PropertyInfo)info).PropertyType;
            default:
                return null;
        }
    }

    private void SetValueOfMember<T>(MemberInfo info, T input, object source)
    {
        switch (info.MemberType)
        {
            case MemberTypes.Field:
                ((FieldInfo)info).SetValue(source, input);
                break;
            case MemberTypes.Property:
                ((PropertyInfo)info).SetValue(source, input, null);
                break;
            default:
                break;
        }
    }

    private T GetValueOfMember<T>(MemberInfo info, object source)
    {
        switch (info.MemberType) {
            case MemberTypes.Field:
                return (T)((FieldInfo)info).GetValue(source);
            case MemberTypes.Property:
                return (T)((PropertyInfo)info).GetValue(source, null);
            default:
                return default(T);
        }
    }

    private MemberInfo GetMemberByName(Component component, string observedMember)
    {
        MemberInfo info = component.GetType().GetField(observedMember);
        if (info == null)
        {
            info = component.GetType().GetProperty(observedMember);
        }
        return info;
    }
}

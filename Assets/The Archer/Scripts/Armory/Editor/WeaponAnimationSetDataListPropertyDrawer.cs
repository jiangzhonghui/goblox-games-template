using OctoberStudio.Armory;
using OctoberStudio.StageCreator;
using UnityEditor;
using UnityEngine;

namespace OctoberStudio
{
    [CustomPropertyDrawer(typeof(WeaponAnimationSetDataList))]
    public class WeaponAnimationSetDataListPropertyDrawer : PropertyDrawer
    {
        protected static float WeaponsRowHeight = 50f;
        protected static float WeaponColumnWidth = 100f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var serializedObject = property.serializedObject;

            if (serializedObject.targetObject is ArmoryDatabase database)
            {
                var arrayProperty = property.FindPropertyRelative("weaponAnimationSets");

                var weaponsCount = 0;
                for (int i = 0; i < database.ItemsCount; i++)
                {
                    var weapon = database.Items[i];
                    if (weapon == null || weapon.ItemType != ItemType.Weapon) continue;

                    var weaponLabelRect = new Rect(position.x + 100 + WeaponColumnWidth * (weaponsCount), position.y, WeaponColumnWidth, 50);

                    GUI.Label(weaponLabelRect, weapon.ItemName);

                    var separatorRect = new Rect(weaponLabelRect);
                    separatorRect.height = position.height;
                    separatorRect.width = 1;

                    EditorGUI.DrawRect(separatorRect, Color.black);
                    weaponsCount++;
                }

                {
                    var separatorRect = new Rect(position.x + 100 + WeaponColumnWidth * (weaponsCount), position.y, 1, position.height);
                    EditorGUI.DrawRect(separatorRect, Color.black);
                }

                var width = GetWidth(position, property);

                var heroesCount = 0;
                for (int i = 0; i < database.HeroesCount; i++)
                {
                    var hero = database.Heroes[i];
                    if (hero == null) continue;
                    var heroId = hero.Id;

                    weaponsCount = 0;

                    var rowRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * heroesCount + WeaponsRowHeight, position.width, EditorGUIUtility.singleLineHeight);
                    var heroLabelRect = new Rect(rowRect.x, rowRect.y, 100, rowRect.height);
                    GUI.Label(heroLabelRect, hero.Name);

                    var separatorRect = new Rect(heroLabelRect);
                    separatorRect.height = 1f;
                    separatorRect.width = width;

                    EditorGUI.DrawRect(separatorRect, Color.black);

                    for (int j = 0; j < database.ItemsCount; j++)
                    {
                        var weapon = database.Items[j];
                        if (weapon.ItemType != ItemType.Weapon) continue;

                        var weaponId = weapon.Id;

                        var dataProperty = GetOrCreateAnimationSetDataProperty(arrayProperty, heroId, weaponId);
                        if (dataProperty != null)
                        {
                            var setProperty = dataProperty.FindPropertyRelative("animationsSet");

                            var rect = new Rect(
                                position.x + 100 + WeaponColumnWidth * weaponsCount,
                                position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * heroesCount + WeaponsRowHeight,
                                WeaponColumnWidth,
                                EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                            rect = rect.Shrink(2f);
                            setProperty.objectReferenceValue = EditorGUI.ObjectField(rect, setProperty.objectReferenceValue, typeof(WeaponAnimationsSet), false);
                        }

                        weaponsCount++;
                    }

                    heroesCount++;
                }

                {
                    var separatorRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * heroesCount + WeaponsRowHeight, width, 1);
                    EditorGUI.DrawRect(separatorRect, Color.black);
                }
            }
            else
            {
                base.OnGUI(position, property, label);
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected SerializedProperty GetOrCreateAnimationSetDataProperty(SerializedProperty arrayProperty, string heroId, string weaponId)
        {
            SerializedProperty dataProperty = null;

            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                var item = arrayProperty.GetArrayElementAtIndex(i);
                var weaponIdProperty = item.FindPropertyRelative("weaponId").stringValue;
                var heroIdProperty = item.FindPropertyRelative("heroId").stringValue;

                if (weaponIdProperty == weaponId && heroIdProperty == heroId)
                {
                    if (dataProperty != null)
                    {
                        arrayProperty.DeleteArrayElementAtIndex(i);
                        i--;
                        continue;
                    }
                    else
                    {
                        dataProperty = item;
                    }
                }
            }

            if (dataProperty == null)
            {
                arrayProperty.arraySize++;
                dataProperty = arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1);

                dataProperty.FindPropertyRelative("weaponId").stringValue = weaponId;
                dataProperty.FindPropertyRelative("heroId").stringValue = heroId;
                dataProperty.FindPropertyRelative("animationsSet").objectReferenceValue = null;
            }

            return dataProperty;
        }

        protected virtual float GetWidth(Rect position, SerializedProperty property)
        {
            var serializedObject = property.serializedObject;

            if (serializedObject.targetObject is ArmoryDatabase database)
            {
                var weaponsCount = 0;

                for (int i = 0; i < database.ItemsCount; i++)
                {
                    var weapon = database.Items[i];
                    if (weapon.ItemType != ItemType.Weapon) continue;

                    weaponsCount++;
                }
                return weaponsCount * WeaponColumnWidth + 100;
            }

            return position.width;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var serializedObject = property.serializedObject;

            if (serializedObject.targetObject is ArmoryDatabase database)
            {
                return database.HeroesCount * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * (database.HeroesCount - 1) + WeaponsRowHeight;
            }

            return base.GetPropertyHeight(property, label);
        }
    }
}
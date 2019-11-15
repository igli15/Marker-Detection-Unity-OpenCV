﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RuntimeInspectorNamespace
{
	public class RuntimeInspector : SkinnedWindow
	{
		public delegate object InspectedObjectChangingDelegate( object previousInspectedObject, object newInspectedObject );
		public delegate void ComponentFilterDelegate( GameObject gameObject, List<Component> components );

		private const string POOL_OBJECT_NAME = "RuntimeInspectorPool";

		private object m_inspectedObject;
		public object InspectedObject { get { return m_inspectedObject; } }

		public bool IsBound { get { return !m_inspectedObject.IsNull(); } }

#pragma warning disable 0649
		[SerializeField]
		private float refreshInterval = 0f;
		private float nextRefreshTime = -1f;

		[SerializeField]
		private bool m_debugMode = true;
		public bool DebugMode
		{
			get { return m_debugMode; }
			set
			{
				if( m_debugMode != value )
				{
					m_debugMode = value;
					isDirty = true;
				}
			}
		}

		[SerializeField]
		private bool m_exposePrivateFields = true;
		public bool ExposePrivateFields
		{
			get { return m_exposePrivateFields; }
			set
			{
				if( m_exposePrivateFields != value )
				{
					m_exposePrivateFields = value;
					isDirty = true;
				}
			}
		}

		[SerializeField]
		private bool m_exposePublicFields = true;
		public bool ExposePublicFields
		{
			get { return m_exposePublicFields; }
			set
			{
				if( m_exposePublicFields != value )
				{
					m_exposePublicFields = value;
					isDirty = true;
				}
			}
		}

		[SerializeField]
		private bool m_exposePrivateProperties = true;
		public bool ExposePrivateProperties
		{
			get { return m_exposePrivateProperties; }
			set
			{
				if( m_exposePrivateProperties != value )
				{
					m_exposePrivateProperties = value;
					isDirty = true;
				}
			}
		}

		[SerializeField]
		private bool m_exposePublicProperties = true;
		public bool ExposePublicProperties
		{
			get { return m_exposePublicProperties; }
			set
			{
				if( m_exposePublicProperties != value )
				{
					m_exposePublicProperties = value;
					isDirty = true;
				}
			}
		}

		[SerializeField]
		private bool m_arrayIndicesStartAtOne = false;
		public bool ArrayIndicesStartAtOne
		{
			get { return m_arrayIndicesStartAtOne; }
			set
			{
				if( m_arrayIndicesStartAtOne != value )
				{
					m_arrayIndicesStartAtOne = value;
					isDirty = true;
				}
			}
		}

		[SerializeField]
		private bool m_useTitleCaseNaming = false;
		public bool UseTitleCaseNaming
		{
			get { return m_useTitleCaseNaming; }
			set
			{
				if( m_useTitleCaseNaming != value )
				{
					m_useTitleCaseNaming = value;
					isDirty = true;
				}
			}
		}

		[SerializeField]
		private int m_nestLimit = 5;
		public int NestLimit
		{
			get { return m_nestLimit; }
			set
			{
				if( m_nestLimit != value )
				{
					m_nestLimit = value;
					isDirty = true;
				}
			}
		}

		[SerializeField]
		private int poolCapacity = 10;
		private Transform poolParent;
		private static int aliveInspectors = 0;
		private static Dictionary<Type, List<InspectorField>> drawersPool = new Dictionary<Type, List<InspectorField>>();

		[SerializeField]
		private RuntimeHierarchy m_connectedHierarchy;
		public RuntimeHierarchy ConnectedHierarchy
		{
			get { return m_connectedHierarchy; }
			set { m_connectedHierarchy = value; }
		}

		[SerializeField]
		private RuntimeInspectorSettings[] settings;

		[Header( "Internal Variables" )]
		[SerializeField]
		private ScrollRect scrollView;
		private RectTransform drawArea;

		[SerializeField]
		private Image background;

		[SerializeField]
		private Image scrollbar;
#pragma warning restore 0649

		private InspectorField currentDrawer = null;
		private bool inspectLock = false;
		private bool isDirty = false;

		public InspectedObjectChangingDelegate OnInspectedObjectChanging;

		private ComponentFilterDelegate m_componentFilter;
		public ComponentFilterDelegate ComponentFilter
		{
			get { return m_componentFilter; }
			set
			{
				m_componentFilter = value;
				Refresh();
			}
		}

		private Dictionary<Type, InspectorField> typeToDrawer = new Dictionary<Type, InspectorField>( 89 );
		private Dictionary<Type, InspectorField> typeToReferenceDrawer = new Dictionary<Type, InspectorField>( 89 );

		private List<VariableSet> hiddenVariables = new List<VariableSet>( 32 );
		private List<VariableSet> exposedVariables = new List<VariableSet>( 32 );

		protected override void Awake()
		{
			base.Awake();

			drawArea = scrollView.content;

			GameObject poolParentGO = GameObject.Find( POOL_OBJECT_NAME );
			if( poolParentGO == null )
			{
				poolParentGO = new GameObject( POOL_OBJECT_NAME );
				DontDestroyOnLoad( poolParentGO );
			}

			poolParent = poolParentGO.transform;
			aliveInspectors++;

			for( int i = 0; i < settings.Length; i++ )
			{
				VariableSet[] hiddenVariablesForTypes = settings[i].HiddenVariables;
				for( int j = 0; j < hiddenVariablesForTypes.Length; j++ )
				{
					VariableSet hiddenVariablesSet = hiddenVariablesForTypes[j];
					if( hiddenVariablesSet.Init() )
						hiddenVariables.Add( hiddenVariablesSet );
				}

				VariableSet[] exposedVariablesForTypes = settings[i].ExposedVariables;
				for( int j = 0; j < exposedVariablesForTypes.Length; j++ )
				{
					VariableSet exposedVariablesSet = exposedVariablesForTypes[j];
					if( exposedVariablesSet.Init() )
						exposedVariables.Add( exposedVariablesSet );
				}
			}

			RuntimeInspectorUtils.IgnoredTransformsInHierarchy.Add( drawArea );
			RuntimeInspectorUtils.IgnoredTransformsInHierarchy.Add( poolParent );

			ColorPicker.Instance.Close();
			ObjectReferencePicker.Instance.Close();
		}

		private void OnDestroy()
		{
			if( --aliveInspectors == 0 )
			{
				if( !poolParent.IsNull() )
				{
					RuntimeInspectorUtils.IgnoredTransformsInHierarchy.Remove( poolParent );
					DestroyImmediate( poolParent.gameObject );
				}

				drawersPool.Clear();
			}

			RuntimeInspectorUtils.IgnoredTransformsInHierarchy.Remove( drawArea );
		}

		protected override void Update()
		{
			base.Update();

			if( IsBound )
			{
				if( isDirty )
				{
					// Rebind to refresh the exposed variables in Inspector
					object inspectedObject = m_inspectedObject;
					StopInspect();
					Inspect( inspectedObject );

					isDirty = false;
					nextRefreshTime = Time.realtimeSinceStartup + refreshInterval;
				}
				else
				{
					if( Time.realtimeSinceStartup > nextRefreshTime )
					{
						nextRefreshTime = Time.realtimeSinceStartup + refreshInterval;
						Refresh();
					}
				}
			}
			else if( currentDrawer != null )
				StopInspect();
		}

		public void Refresh()
		{
			if( IsBound )
			{
				if( currentDrawer == null )
					m_inspectedObject = null;
				else
					currentDrawer.Refresh();
			}
		}

		protected override void RefreshSkin()
		{
			background.color = Skin.BackgroundColor;
			scrollbar.color = Skin.ScrollbarColor;

			if( IsBound && !isDirty )
				currentDrawer.Skin = Skin;
		}

		public void Inspect( object obj )
		{
			if( inspectLock )
				return;

			isDirty = false;

			if( OnInspectedObjectChanging != null )
				obj = OnInspectedObjectChanging( m_inspectedObject, obj );

			if( m_inspectedObject == obj )
				return;

			StopInspect();

			inspectLock = true;
			try
			{
				m_inspectedObject = obj;

				if( obj.IsNull() )
					return;

#if UNITY_EDITOR || !NETFX_CORE
				if( obj.GetType().IsValueType )
#else
				if( obj.GetType().GetTypeInfo().IsValueType )
#endif
				{
					m_inspectedObject = null;
					Debug.LogError( "Can't inspect a value type!" );
					return;
				}

				if( !gameObject.activeSelf )
				{
					m_inspectedObject = null;
					Debug.LogError( "Can't inspect while Inspector is inactive!" );
					return;
				}

				InspectorField inspectedObjectDrawer = CreateDrawerForType( obj.GetType(), drawArea, 0, false );
				if( inspectedObjectDrawer != null )
				{
					inspectedObjectDrawer.BindTo( obj.GetType(), string.Empty, () => m_inspectedObject, ( value ) => m_inspectedObject = value );
					inspectedObjectDrawer.NameRaw = obj.GetNameWithType();
					inspectedObjectDrawer.Refresh();

					if( inspectedObjectDrawer is ExpandableInspectorField )
						( (ExpandableInspectorField) inspectedObjectDrawer ).IsExpanded = true;

					currentDrawer = inspectedObjectDrawer;

					GameObject go = m_inspectedObject as GameObject;
					if( go == null && m_inspectedObject is Component )
						go = ( (Component) m_inspectedObject ).gameObject;

					if( !ConnectedHierarchy.IsNull() && ( go == null || !ConnectedHierarchy.Select( go.transform ) ) )
						ConnectedHierarchy.Deselect();
				}
				else
					m_inspectedObject = null;
			}
			finally
			{
				inspectLock = false;
			}
		}

		public void StopInspect()
		{
			if( inspectLock )
				return;

			if( currentDrawer != null )
			{
				currentDrawer.Unbind();
				currentDrawer = null;
			}

			m_inspectedObject = null;
			scrollView.verticalNormalizedPosition = 1f;

			ColorPicker.Instance.Close();
			ObjectReferencePicker.Instance.Close();
		}

		public InspectorField CreateDrawerForType( Type type, Transform drawerParent, int depth, bool drawObjectsAsFields = true )
		{
			InspectorField variableDrawer = GetDrawerForType( type, drawObjectsAsFields );
			if( variableDrawer != null )
			{
				InspectorField drawer = InstantiateDrawer( variableDrawer, drawerParent );
				drawer.Inspector = this;
				drawer.Skin = Skin;
				drawer.Depth = depth;

				return drawer;
			}

			return null;
		}

		private InspectorField InstantiateDrawer( InspectorField drawer, Transform drawerParent )
		{
			List<InspectorField> drawerPool;
			if( drawersPool.TryGetValue( drawer.GetType(), out drawerPool ) )
			{
				for( int i = drawerPool.Count - 1; i >= 0; i-- )
				{
					InspectorField instance = drawerPool[i];
					drawerPool.RemoveAt( i );

					if( instance != null && !instance.Equals( null ) )
					{
						instance.transform.SetParent( drawerParent, false );
						instance.gameObject.SetActive( true );

						return instance;
					}
				}
			}

			InspectorField newDrawer = (InspectorField) Instantiate( drawer, drawerParent, false );
			newDrawer.Initialize();
			return newDrawer;
		}

		private InspectorField GetDrawerForType( Type type, bool drawObjectsAsFields )
		{
			bool searchReferenceFields = drawObjectsAsFields && typeof( Object ).IsAssignableFrom( type );

			InspectorField cachedResult;
			if( ( searchReferenceFields && typeToReferenceDrawer.TryGetValue( type, out cachedResult ) ) ||
				( !searchReferenceFields && typeToDrawer.TryGetValue( type, out cachedResult ) ) )
				return cachedResult;

			Dictionary<Type, InspectorField> drawersDict = searchReferenceFields ? typeToReferenceDrawer : typeToDrawer;

			for( int i = settings.Length - 1; i >= 0; i-- )
			{
				InspectorField[] drawers = searchReferenceFields ? settings[i].ReferenceDrawers : settings[i].StandardDrawers;
				for( int j = drawers.Length - 1; j >= 0; j-- )
				{
					if( drawers[j].SupportsType( type ) )
					{
						drawersDict[type] = drawers[j];
						return drawers[j];
					}
				}
			}

			drawersDict[type] = null;
			return null;
		}

		public void PoolDrawer( InspectorField drawer )
		{
			List<InspectorField> drawerPool;
			if( !drawersPool.TryGetValue( drawer.GetType(), out drawerPool ) )
			{
				drawerPool = new List<InspectorField>( poolCapacity );
				drawersPool[drawer.GetType()] = drawerPool;
			}

			if( drawerPool.Count < poolCapacity )
			{
				drawer.gameObject.SetActive( false );
				drawer.transform.SetParent( poolParent, false );
				drawerPool.Add( drawer );
			}
			else
				Destroy( drawer.gameObject );
		}

		public ExposedVariablesEnumerator GetExposedVariablesForType( Type type )
		{
			MemberInfo[] allVariables = type.GetAllVariables();
			if( allVariables == null )
				return new ExposedVariablesEnumerator( null, null, null, false, false, false, false, false );

			List<VariableSet> hiddenVariablesForType = null;
			List<VariableSet> exposedVariablesForType = null;
			for( int i = 0; i < hiddenVariables.Count; i++ )
			{
				if( hiddenVariables[i].type.IsAssignableFrom( type ) )
				{
					if( hiddenVariablesForType == null )
						hiddenVariablesForType = new List<VariableSet>() { hiddenVariables[i] };
					else
						hiddenVariablesForType.Add( hiddenVariables[i] );
				}
			}

			for( int i = 0; i < exposedVariables.Count; i++ )
			{
				if( exposedVariables[i].type.IsAssignableFrom( type ) )
				{
					if( exposedVariablesForType == null )
						exposedVariablesForType = new List<VariableSet>() { exposedVariables[i] };
					else
						exposedVariablesForType.Add( exposedVariables[i] );
				}
			}

			return new ExposedVariablesEnumerator( allVariables, hiddenVariablesForType, exposedVariablesForType, m_debugMode,
				m_exposePrivateFields, m_exposePublicFields, m_exposePrivateProperties, m_exposePublicProperties );
		}
	}
}
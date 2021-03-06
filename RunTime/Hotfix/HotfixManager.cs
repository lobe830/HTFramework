﻿using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 热更新模块管理者
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Hotfix)]
    public sealed class HotfixManager : InternalModuleBase
    {
        /// <summary>
        /// 是否启用热更新【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsEnableHotfix = false;
        /// <summary>
        /// 热更新库文件AB包名称【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string HotfixDllAssetBundleName = "hotfix";
        /// <summary>
        /// 热更新库文件路径【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string HotfixDllAssetsPath = "Assets/Hotfix/Hotfix.dll.bytes";

        /// <summary>
        /// 执行热更新逻辑事件
        /// </summary>
        public event HTFAction UpdateHotfixLogicEvent;

        private TextAsset _hotfixDll;
        private Assembly _hotfixAssembly;
        private object _hotfixEnvironment;

        public override void OnPreparatory()
        {
            base.OnPreparatory();

            if (IsEnableHotfix)
            {
                if (Main.m_Resource.Mode == ResourceLoadMode.Resource)
                {
                    throw new HTFrameworkException(HTFrameworkModule.Hotfix, "热更新初始化失败：热更新库不支持使用Resource加载模式！");
                }

                AssetInfo info = new AssetInfo(HotfixDllAssetBundleName, HotfixDllAssetsPath, "");
                Main.m_Resource.LoadAsset<TextAsset>(info, null, HotfixDllLoadDone);
            }
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            if (IsEnableHotfix)
            {
                UpdateHotfixLogicEvent?.Invoke();
            }
        }

        private void HotfixDllLoadDone(TextAsset asset)
        {
            _hotfixDll = asset;
            _hotfixAssembly = Assembly.Load(_hotfixDll.bytes, null);
            _hotfixEnvironment = _hotfixAssembly.CreateInstance("HotfixEnvironment");

            if (_hotfixEnvironment == null)
            {
                throw new HTFrameworkException(HTFrameworkModule.Hotfix, "热更新初始化失败：热更新库中不存在热更新环境 HotfixEnvironment！");
            }
        }
    }
}
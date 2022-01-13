using Avalonia.Controls;
using Plat._C;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    public class MainWindow_VM : ViewModelBase
    {
        private string? tip = "Welcome to SharpMS!";
        private readonly MetaInfo_PG_VM metaInfo_PG_VM = new MetaInfo_PG_VM();
        private readonly ClassDiagram_P_VM classDiagram_P_VM = new ClassDiagram_P_VM();
        private readonly ProcGraph_PG_VM procGraph_PG_VM = new ProcGraph_PG_VM();
        private readonly TopoGraph_P_VM topoGraph_P_VM = new TopoGraph_P_VM();
        private bool anchorVisible;

        public MainWindow_VM()
        {
            ResourceManager.mainWindow_VM = this;
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        public string? Tip { get => tip; set => this.RaiseAndSetIfChanged(ref tip, value); }
        /// <summary>
        /// MetaInfo�����
        /// </summary>
        public MetaInfo_PG_VM MetaInfo_PG_VM => metaInfo_PG_VM;
        /// <summary>
        /// ClassDiagram���
        /// </summary>
        public ClassDiagram_P_VM ClassDiagram_P_VM => classDiagram_P_VM;
        /// <summary>
        /// ProcGraph�����
        /// </summary>
        public ProcGraph_PG_VM ProcGraph_PG_VM => procGraph_PG_VM;
        /// <summary>
        /// TopoGraph���
        /// </summary>
        public TopoGraph_P_VM TopoGraph_P_VM => topoGraph_P_VM;
        /// <summary>
        /// ���ڰ�������ϵ�AnchorVisible��CheckBox��IsChecked����
        /// �ڱ��ʱ���ﴥ����Ȼ��ȥ����ResourceManager��anchorVisible�Ķ���
        /// Ȼ�����е�ê��Anchor_V�ڹ���ʱ�򶼰����Ǹ����ģ����Կ����յ�֪ͨ
        /// </summary>
        public bool AnchorVisible
        {
            get => anchorVisible;
            set
            {
                anchorVisible = value;
                ResourceManager.anchorVisible.OnNext(value);
            }
        }

        #region Command

        /// <summary>
        /// ��� Load����ģ���������
        /// </summary>
        private async Task OnLoad()
        {
            // �û�������ȡ�Ի��򲢷��ض�ȡ�ļ���
            string loadPath = await PersistenceManager.OpenDialogAndGetLoadPathFromUser();
            if (string.IsNullOrEmpty(loadPath))
            {
                ResourceManager.UpdateTip("Cancel loading operation.");
                return;
            }
            bool loadRes = PersistenceManager.LoadProjectModelFromXmlFile(loadPath);
            if (!loadRes)
            {
                ResourceManager.UpdateTip($"Fail to load model from path = {loadPath}");
                return;
            }
            ResourceManager.UpdateTip($"Successfully load model from path = {loadPath}");
        }

        /// <summary>
        /// ��� Save As�������Ϊ����
        /// </summary>
        /// <returns></returns>
        private async Task OnSaveAs()
        {
            // �û���������Ի��򲢷��ش�����λ��
            string savePath = await PersistenceManager.OpenDialogAndGetSavePathFromUser();
            if (string.IsNullOrEmpty(savePath))
            {
                ResourceManager.UpdateTip("Cancel saving operation.");
                return;
            }
            bool saveRes = PersistenceManager.SaveProjectModelAsXmlFile(savePath);
            if (!saveRes)
            {
                ResourceManager.UpdateTip($"Fail to save model with path = {savePath}");
                return;
            }
            ResourceManager.UpdateTip($"Successfully save model with path = {savePath}");
        }
        
        /// <summary>
        /// ��� About SharpMS
        /// </summary>
        private void OnAboutSharpMS()
        {
            ResourceManager.UpdateTip("Get more info at https://github.com/LauZyHou/SharpMS");
        }

        /// <summary>
        /// ��� Translate -> To UPPAAL
        /// </summary>
        private void OnTransToUPPAAL()
        {

        }

        /// <summary>
        /// ��� Translate -> To ProVerif
        /// </summary>
        private void OnTransToProVerif()
        {

        }

        #endregion
    }
}

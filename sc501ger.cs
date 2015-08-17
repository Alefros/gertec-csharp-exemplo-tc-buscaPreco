using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Aplicação_TC506
{
    class sc501ger
    {
        //Importa todas as funções da DLL
        
        [DllImport("SC501GER.dll", EntryPoint = "vInitialize")]
        public static extern void vInitialize();

        [DllImport("SC501GER.dll", EntryPoint = "vFinalize")]
        public static extern void vFinalize();

        [DllImport("SC501GER.dll", EntryPoint = "dll_version")]
        public static extern int dll_version();

        [DllImport("SC501GER.dll", EntryPoint = "tc_startserver")]
        public static extern int tc_startserver();

        [DllImport("SC501GER.dll", EntryPoint = "GetTermConectados")]
        public static extern void GetTermConectados(int nada, ref TTABTERM ptabTerm);

        [DllImport("SC501GER.dll", EntryPoint = "TCinet_ntoa")]
        public static extern void TCinet_ntoa(uint nIP, byte[] buf);

        [DllImport("SC501GER.dll", EntryPoint = "bMandaConfig")]
        public static extern bool bMandaConfig(ref TCCONFIG mandaConfiguracao);

        [DllImport("SC501GER.dll", EntryPoint = "bMandaParam")]
        public static extern bool bMandaParam(ref TCPARAMCONFIG mandaParametro);

        [DllImport("SC501GER.dll", EntryPoint = "bMandaWlanConfig")]
        public static extern bool bMandaWlanConfig(ref TCWLANCONFIG mandaWlanConfig);

        [DllImport("SC501GER.dll", EntryPoint = "bReceiveConfig")]
        public static extern bool bReceiveConfig(ref TCCONFIG PTCCONFIG);

        [DllImport("SC501GER.dll", EntryPoint = "bReceiveBarcode")]
        public static extern bool bReceiveBarcode(int[] id, int[] porta,byte[] buffer, int[] Nbr);

        [DllImport("SC501GER.dll", EntryPoint = "bReceiveWlanConfig")]
        public static extern int bReceiveWlanConfig(ref TCWLANCONFIG recebeWLanConfig);

        [DllImport("SC501GER.dll", EntryPoint = "bReceiveParam")]
        public static extern int bReceiveParam(ref TCPARAMCONFIG ParamConfig);

        [DllImport("SC501GER.dll", EntryPoint = "bReceiveMacAddrConfig")]
        public static extern byte[] bReceiveMacAddrConfig();

        /*NESTA CHAMADA, EXISTEM PARAMETROS NÃO INFORMADOS NO MANUAL
                function bSendImageFromFile(ID: Integer; var filename : byte; width, height, index, loop, pause, resize : integer)
                loop: quantidade de vezes em que é exibida de forma seguida
                pause: tempo entre troca de imagens
                resize: se precisa redimencionar a imagem (manda 0)
         */

        [DllImport("SC501GER.dll", EntryPoint = "bSendImageFromFile")]
        public static extern bool bSendImageFromFile(int id, byte[] filename, int width, int height, int index, int loop, int pause, int resize);

        [DllImport("SC501GER.dll", EntryPoint = "bSendImagePrice")]
        public static extern void bSendImagePrice(int id, byte nomeProduto,byte precoProduto,int tempo, int width, int height);

        [DllImport("SC501GER.dll", EntryPoint = "bSendRestartSoft")]
        public static extern void bSendRestartSoft(int id);
        
        [DllImport("SC501GER.dll", EntryPoint = "bSendDisplayMsg")]
        public static extern bool bSendDisplayMsg(int ID, byte[] Line1, byte[] Line2, int TimeExhibition, int TypeAnimation);

        [DllImport("SC501GER.dll", EntryPoint = "bSendProdNotFound")]
        public static extern int bSendProdNotFound(int id);

        [DllImport("SC501GER.dll", EntryPoint = "bSendProdPrice")]
        public static extern int bSendProdPrice(int id, byte[] NameProd, byte[] PriceProd);

        [DllImport("SC501GER.dll", EntryPoint = "bPedeConfig")]
        public static extern bool bPedeConfig(int id);

        [DllImport("SC501GER.dll", EntryPoint = "bPedeParam")]
        public static extern bool bPedeParam(int id);

        [DllImport("SC501GER.dll", EntryPoint = "bPedeWlanConfig")]
        public static extern bool bPedeWlanConfig(int id);

        [DllImport("SC501GER.dll", EntryPoint = "bPedeMacAddr")]
        public static extern bool bPedeMacAddr(int id, byte iface);

    }

    public struct informacoesImagem
    {
        public string localizacaoArquivo;
        public string nomeArquivo;
        public int indiceSequencial;
        public int repeticoes;
        public int pausa;
    }

    public struct TCPARAMCONFIG
    {
        public int id;
        public byte ipdinamico;
        public byte buscaserv;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct TCWLANCONFIG
    {
        public int ID; 
        public byte usewifi;
        public byte mode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public byte[] ssid;
        public byte channel;
        public byte wep;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public byte[] wepkey;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TTABTERM
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8704)]
        public byte[] TabName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public int[] TabSock;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public uint[] TabIP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public int[] Tipo;
        public int NumSockConec;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct TCCONFIG
    {
        public int ID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public byte[] host;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public byte[] endereco;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public byte[] msknet;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public byte[] texto1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public byte[] texto2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public byte[] texto3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public byte[] texto4;
        public byte tempoExib;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TCMACADDRCONFIG
    {
        public int ID;
        public byte iface;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public byte[] MacAddres;
    }
}
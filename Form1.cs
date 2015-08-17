using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Aplicação_TC506
{



    public partial class Form1 : Form
    {
        public TCCONFIG configRecebida = new TCCONFIG();
        public TTABTERM lista_terminais = new TTABTERM();
        public TCCONFIG receberConfiguracao = new TCCONFIG();
        public TCPARAMCONFIG receberParametros = new TCPARAMCONFIG();
        public TCPARAMCONFIG mandarParametros = new TCPARAMCONFIG();
        public TCMACADDRCONFIG receberMacAdd = new TCMACADDRCONFIG();
        public TCWLANCONFIG receberWlanConfig = new TCWLANCONFIG();
        public TCWLANCONFIG mandarWlanConfig = new TCWLANCONFIG();

        public informacoesImagem imagem1 = new informacoesImagem();
        public informacoesImagem imagem2 = new informacoesImagem();
        public informacoesImagem imagem3 = new informacoesImagem();
        public informacoesImagem imagem4 = new informacoesImagem();

        public int idTerminalSelecionado;
        public int[] filaDeImagens = new int[4];
        public int tempoAcumulado = 0;
        public long acumulo = 0;

        public string mensagemLog;
        public string tipoMensagemLog;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Inicializa a DLL, o Servidor TCm os timers...
            sc501ger.vInitialize();
            sc501ger.tc_startserver();
            timer1.Start();
            timer2.Start();
            dgProdutos.Rows.Add("1112223331119", "Café", "10,00"); //Carrega o café na lista de produtos
            txtLine2.Text = "Linha 2";
           


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {

                TTABTERM lista_terminais = new TTABTERM();
                sc501ger.GetTermConectados(1, ref lista_terminais);                          //Obtém a lista de terminais conectados
                int quantidadeTerminaisListados = lista_terminais_conectados.Items.Count;   //armazenar quantos terminais estão na lista
                int quantidadeTerminaisConectados = lista_terminais.NumSockConec;           //armazenar quantos terminais estão conectados 

                int controle = 0;
                if (lista_terminais.NumSockConec > 0)                                       //Carrega os terminais conectados em uma lista sequencialmente
                {
                    while (controle != lista_terminais.NumSockConec)                        //controla se todos os terminais foram conectados
                    {
                        byte[] ip_convertido = new byte[20];
                        string ip = "";
                        sc501ger.TCinet_ntoa(lista_terminais.TabIP[controle], ip_convertido);   //Recebe da DLL o IP do terminal formatado com pontos "pontos"
                        ip = ((System.Text.Encoding.UTF8.GetString(ip_convertido)).Replace("\0", "")).Trim(); //Remove os "Zeros" ao fim do IP
                        if (!lista_terminais_conectados.Items.Contains(ip))                                    //Verifica se o IP já está na lista 
                        {
                            lista_terminais_conectados.Items.Add(ip);                                           //Caso não esteja adiciona na lista
                            quantidadeTerminaisListados++;                                                      //Conta o terminal adicionado        
                        }
                        controle++;
                    }
                }
                // Limpar lista dos terminais que foram desconectados
                if (quantidadeTerminaisListados > quantidadeTerminaisConectados) //verifica se existem terminais listados que foram desconectado
                {
                    //Limpa todos os itens da lista, os itens que ainda estão conectados serão conectados novamente na próxima volta do TIMER
                    for (int contador = 1; contador <= quantidadeTerminaisListados; contador++)
                    {
                        lista_terminais_conectados.Items.RemoveAt(contador - 1);
                    }
                }

            }
            catch
            {
                popularLog("ERRO", "Ocorreu um erro durante a operação");
            }
        }
        private void txtMsg1_MouseUp(object sender, MouseEventArgs e)
        {
            /*Ao cliclar na caixa de texto, caso seja o texto default("Mensagem 1 "), limpa este texto e retira o aspecto esmaecido*/
            if (txtMsg1.Text == "Mensagem 1")
            {
                txtMsg1.Text = "";
                txtMsg1.ForeColor = Color.Black;
            }
        }
        private void txtEnviarMsgRap_Click(object sender, EventArgs e)
        {
            try
            {
                sc501ger.GetTermConectados(1, ref lista_terminais);
                if (validarConectadoSelecionado()) //Se a função retornar TRUE, significa que existe um terminal conectado e existe um terminal selecionado 
                {
                    // -- Variáveis -- // 
                    int id, tempoDeExibicao;
                    byte[] line1, line2 = new byte[100];

                    // -- Preencher variáveis que serão usadas para mandar informação para a DLL -- //
                    line1 = System.Text.Encoding.ASCII.GetBytes(txtLine1.Text);
                    line2 = System.Text.Encoding.ASCII.GetBytes(txtLine2.Text);
                    id = lista_terminais.TabSock[lista_terminais_conectados.SelectedIndex];
                    tempoDeExibicao = Convert.ToInt32(txtTimeExib.Text);
                    if (tempoDeExibicao < 0) //-- Define um valor mínimo para o tempo de exibição --//
                    {
                        tempoDeExibicao = 0;
                    }
                    if (tempoDeExibicao >= 256) //-- Define um valor máximo para o tempo de exibição --//
                    {
                        tempoDeExibicao = 255;
                    }
                    sc501ger.bSendDisplayMsg(id, line1, line2, tempoDeExibicao, 48);                                      /*Manda os valores carregados para a DLL*/
                    popularLog("Processo", "Mensagem Enviada Com Sucesso! Verifique O Display Terminal Selecionado" + id);       /*Preenche a Log*/
                }
            }
            catch (Exception erro)
            {
                popularLog("Erro", "Erro Ao Enviar Comando!" + erro);
            }
        }

        private void txtLine1_MouseUp(object sender, MouseEventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtLine1.Text == "Linha 1")
            {
                txtLine1.Text = "";
                txtLine1.ForeColor = Color.Black;
            }
        }

        private void txtLine2_MouseUp(object sender, MouseEventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtLine2.Text == "Linha 2")
            {
                txtLine2.Text = "";
                txtLine2.ForeColor = Color.Black;
            }

        }

        private void txtMsg2_MouseUp(object sender, MouseEventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg2.Text == "Mensagem 2")
            {
                txtMsg2.Text = "";
                txtMsg2.ForeColor = Color.Black;
            }
        }

        private void txtMsg3_MouseUp(object sender, MouseEventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg3.Text == "Mensagem 3")
            {
                txtMsg3.Text = "";
                txtMsg3.ForeColor = Color.Black;
            }
        }

        private void txtMsg4_MouseUp(object sender, MouseEventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg4.Text == "Mensagem 4")
            {
                txtMsg4.Text = "";
                txtMsg4.ForeColor = Color.Black;
            }
        }

        private void txtLine1_Leave(object sender, EventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtLine1.Text == "")
            {
                txtLine1.Text = "Linha 1";
                txtLine1.ForeColor = System.Drawing.SystemColors.WindowFrame;

            }
        }

        private void txtLine2_Leave(object sender, EventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtLine2.Text == "")
            {
                txtLine2.Text = "Linha 2";
                txtLine2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            }
        }

        private void txtMsg1_Leave(object sender, EventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg1.Text == "")
            {
                txtMsg1.Text = "Mensagem 1";
                txtMsg1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            }
        }

        private void txtMsg2_Leave(object sender, EventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg2.Text == "")
            {
                txtMsg2.Text = "Mensagem 2";
                txtMsg2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            }
        }

        private void txtMsg3_Leave(object sender, EventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg3.Text == "")
            {
                txtMsg3.Text = "Mensagem 3";
                txtMsg3.ForeColor = System.Drawing.SystemColors.WindowFrame;
            }
        }

        private void txtMsg4_Leave(object sender, EventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg4.Text == "")
            {
                txtMsg4.Text = "Mensagem 4";
                txtMsg4.ForeColor = System.Drawing.SystemColors.WindowFrame;
            }
        }
        private void btnRecebDoTerm_Click(object sender, EventArgs e)
        {
                try
                {
                    sc501ger.GetTermConectados(1, ref lista_terminais);

                    if(validarConectadoSelecionado())
                    {
                        //Pede configurações do terminal selecionado
                        idTerminalSelecionado = lista_terminais.TabSock[lista_terminais_conectados.SelectedIndex];//Obter o ID (NUM Socket) do terminal selecionado
                        if (sc501ger.bPedeConfig(idTerminalSelecionado)) //Chamar função bPedeConfig para a DLL
                        {
                            popularLog("PROCESSO", "Configurações solicitadas com sucesso"); //Mandar para a log que a função bPedeConfig foi solicitada com sucesso
                            Thread.Sleep(100);
                            sc501ger.bReceiveConfig(ref receberConfiguracao); //Chamar função bReceiveConfig para a DLL, para pegar as informações previamente solicitadas
                            popularLog("PROCESSO", "Configurações recebidas com sucesso");
                            
                            //Preenche as caixas de texto com as informações recebidas
                            txtIdTerm.Text = Convert.ToString(idTerminalSelecionado);
                            txtIPServidor.Text = ASCIIEncoding.Default.GetString(receberConfiguracao.host);
                            txtIpTerminal.Text = ASCIIEncoding.Default.GetString(receberConfiguracao.endereco);
                            txtMascRede.Text = ASCIIEncoding.Default.GetString(receberConfiguracao.msknet);
                            txtMsg1.ForeColor = Color.Black;
                            txtMsg1.Text = ASCIIEncoding.Default.GetString(receberConfiguracao.texto1);
                            txtMsg2.ForeColor = Color.Black;
                            txtMsg2.Text = ASCIIEncoding.Default.GetString(receberConfiguracao.texto2);
                            txtMsg3.ForeColor = Color.Black;
                            txtMsg3.Text = ASCIIEncoding.Default.GetString(receberConfiguracao.texto3);
                            txtMsg4.ForeColor = Color.Black;
                            txtMsg4.Text = ASCIIEncoding.Default.GetString(receberConfiguracao.texto4);
                            txtTexib.Text = Convert.ToString(receberConfiguracao.tempoExib);
                        }
                    }
                }
                catch (Exception erro)
                {
                    popularLog("ERRO", "Configurações solicitadas com erro" + erro);
                }
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            sc501ger.GetTermConectados(1, ref lista_terminais);
            if (validarConectadoSelecionado())
            {
                try
                {
                    //------------------MANDAR CONFIGURAÇÕES PARA TERMINAL------------------//

                    /*
                         As informações são mandadas para  a DLL utilizando a função "bMandaConfig" 
                         - Primeiramente os dados são carregados em uma estrutura definida do tipo TCCONFIG, no caso abaixo chamada de "mandaConfig"
                         depois enviamos uma referencia da estrutura preenchida "sc501ger.bMandaConfig(ref mandaConfig)"
                     */
                    TCCONFIG mandaConfig = new TCCONFIG();      //Declaração da estrutura do tipo "TCCONFIG"
                    byte[] hostConteudo, enderecoConteudo, msknetConteudo, texto1Conteudo, texto2Conteudo, texto3Conteudo, texto4Conteudo;
                    byte[] host, endereco, msknet, texto1, texto2, texto3, texto4 = new byte[22];
                    

                    /*Os vetores de bytes são inicializados com os valores '00' para ocupação de espaço*/
                    host = ASCIIEncoding.Default.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");
                    endereco = ASCIIEncoding.Default.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");
                    msknet = ASCIIEncoding.Default.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");
                    texto1 = ASCIIEncoding.Default.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");
                    texto2 = ASCIIEncoding.Default.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");
                    texto3 = ASCIIEncoding.Default.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");
                    texto4 = ASCIIEncoding.Default.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");
                    
                    /*Os vetores de byte estão recebendo as informações preenchidas nos campos de usuário*/
                    hostConteudo = ASCIIEncoding.Default.GetBytes(txtIPServidor.Text);
                    enderecoConteudo = Encoding.Default.GetBytes(txtIpTerminal.Text);
                    msknetConteudo = Encoding.Default.GetBytes(txtMascRede.Text);
                    texto1Conteudo = ASCIIEncoding.Default.GetBytes(txtMsg1.Text);
                    texto2Conteudo = ASCIIEncoding.Default.GetBytes(txtMsg2.Text);
                    texto2Conteudo = Encoding.Default.GetBytes(txtMsg2.Text);
                    texto3Conteudo = Encoding.Default.GetBytes(txtMsg3.Text);
                    texto4Conteudo = Encoding.Default.GetBytes(txtMsg4.Text);
                    idTerminalSelecionado = lista_terminais.TabSock[lista_terminais_conectados.SelectedIndex];
                    mandaConfig.ID = idTerminalSelecionado;

                    /* O método é chamado passando:
                     * uma referencia de um vetor de bytes não formatado, mas com as informações corretas, Exemplo:'byte[] hostConteudo',
                     * uma referencia de um outro vetor de bytes que receberá o conteudo formatado pelo 'método PreencherByte', Exemplo:'byte[] host';*/
                    preencherByte(ref hostConteudo, ref host, 22);             
                    preencherByte(ref enderecoConteudo, ref endereco, 22);
                    preencherByte(ref msknetConteudo, ref msknet, 22);
                    preencherByte(ref texto1Conteudo, ref texto1, 22);
                    preencherByte(ref texto2Conteudo, ref texto2, 22);
                    preencherByte(ref texto3Conteudo, ref texto3, 22);
                    preencherByte(ref texto4Conteudo, ref texto4, 22);
                    
                    /*Cada campo da estrutura do tipo TCCONFIG é preenchido com o conteúdo dos vetores de byte formatados*/
                    mandaConfig.host = host;            //- IP servidor formatado por pontos;
                    mandaConfig.endereco = endereco;    //- IP terminal formatado por pontos;
                    mandaConfig.msknet = msknet;        //- msknet: mascara de rede formatado por pontos;
                    mandaConfig.texto1 = texto1;        //- texto1/2/3/4: texto das linhas 1/2/3/4;
                    mandaConfig.texto2 = texto2;    
                    mandaConfig.texto3 = texto3;
                    mandaConfig.texto4 = texto4;
                    mandaConfig.tempoExib = Convert.ToByte(txtTexib.Text);      //tempo de exibição dos pares de linhas, convertido para byte
                    sc501ger.bMandaConfig(ref mandaConfig);                     //chamada da função da DLL 'bMandaConfig', passando um ref de uma estrutura TCCONFIG 
                    popularLog("Processo", "Configuração enviada com sucesso"); //manda para log confirmação do processo
                }
                catch (Exception erro)
                {
                    //manda log em caso de erro, manda a descrição do erro lançado
                    popularLog("Erro", Convert.ToString(erro));
                }
            }
        }

        public void preencherByte(ref byte[] recepcao, ref byte[] devolucao, int tamanho)
        {
            /* ----- Este método receberá duas referencias de 'byte[]' (recepção e devolução) e um número tamanho que devolução deverá conter -----*/

            int tamanhoByteRecepcao = recepcao.Length;
            if (tamanhoByteRecepcao == tamanho)
            {
                //verifica se o byte[] recebido já está no tamanho correto
            }
            else
            {
                for (int contador = 0; contador < tamanhoByteRecepcao; contador++)
                {
                    devolucao[contador] = recepcao[contador];
                }
            }
        }

        private void btnMac_Click(object sender, EventArgs e)
        {
            popularLog("ERRO", "Função necessita de um puxadinho, volte depois");
            //MessageBox.Show("");
            /* sc501ger.GetTermConectados(1, ref lista_terminais);
            int idTerminalSelecionado;
            byte iface = new byte();
            iface = 1;
            idTerminalSelecionado = lista_terminais.TabSock[lista_terminais_conectados.SelectedIndex];
            receberMacAdd.ID = idTerminalSelecionado;
            receberMacAdd.iface = iface;
            if (sc501ger.bPedeMacAddr((receberMacAdd.ID), (receberMacAdd.iface)))
            {
 
                Thread.Sleep(100);
               // sc501ger.bReceiveMacAddrConfig(ref receberMacAdd);

               // receberMacAdd 

                receberMacAdd.MacAddres = sc501ger.bReceiveMacAddrConfig();
                txtMac.Text = ASCIIEncoding.Default.GetString(receberMacAdd.MacAddres);
            }*/
        }

        private void txtMsg1_MouseUp_1(object sender, MouseEventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg1.Text == "Mensagem 1")
            {
                txtMsg1.Text = "";
                txtMsg1.ForeColor = Color.Black;
            }
        }

        private void txtMsg2_MouseUp_1(object sender, MouseEventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg2.Text == "Mensagem 2")
            {
                txtMsg2.Text = "";
                txtMsg2.ForeColor = Color.Black;
            }
        }

        private void txtMsg3_MouseUp_1(object sender, MouseEventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg3.Text == "Mensagem 3")
            {
                txtMsg3.Text = "";
                txtMsg3.ForeColor = Color.Black;
            }
        }

        private void txtMsg4_MouseUp_1(object sender, MouseEventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg4.Text == "Mensagem 4")
            {
                txtMsg4.Text = "";
                txtMsg4.ForeColor = Color.Black;
            }
        }

        private void txtMsg1_Leave_1(object sender, EventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg1.Text == "")
            {
                txtMsg1.Text = "Mensagem 1";
                txtMsg1.ForeColor = System.Drawing.SystemColors.WindowFrame;

            }
        }

        private void txtMsg2_Leave_1(object sender, EventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg2.Text == "")
            {
                txtMsg2.Text = "Mensagem 2";
                txtMsg2.ForeColor = System.Drawing.SystemColors.WindowFrame;

            }
        }

        private void txtMsg3_Leave_1(object sender, EventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg3.Text == "")
            {
                txtMsg3.Text = "Mensagem 3";
                txtMsg3.ForeColor = System.Drawing.SystemColors.WindowFrame;

            }
        }

        private void txtMsg4_Leave_1(object sender, EventArgs e)
        {
            /*Apaga o texto Padrão e tira o aspecto de esmaecido*/
            if (txtMsg4.Text == "")
            {
                txtMsg4.Text = "Mensagem 4";
                txtMsg4.ForeColor = System.Drawing.SystemColors.WindowFrame;
            }
        }

        private void btnExpandir_Click(object sender, EventArgs e)
        {
            /*
                 Aumenta a altura do componente 'lblog' para que ele fique alinhado à 'lista_de_terminais_conectados' 
                 ou 
                 Diminui a altura do componente para que ele fique na altura padrão
             */

            Point localizacaoLista = new Point(6, 20);
            Point localizacaoExpandidaLista = new Point(6, 19);
            Point localizacaoGB = new Point(176, 433);
            Point localizacaoExpandidaGB = new Point(176, 3);


             /*
                Se o texto do botão estiver como "Expandir LOG"
                Significa que o componente está com tamanho padrão 
                Então deverá ser expandido e mudar o texto para "Encolher"
             */

            if (btnExpandir.Text.Equals("Expandir"))
            {
                btnExpandir.Text = "Encolher";
                lbLog.Height = 641;
                lbLog.Location = localizacaoExpandidaLista;
                gbLog.Height = 674;
                gbLog.Location = localizacaoExpandidaGB;
            }
            /*
                Se o texto do botão estiver como "Encolher LOG"
                Significa que o componente está com tamanho expandido 
                Então deverá ser encolhido e mudar o texto para "Expandir Log"
            */
            else if (btnExpandir.Text.Equals("Encolher"))
            {
                btnExpandir.Text = "Expandir";
                lbLog.Height = 212;
                lbLog.Location = localizacaoLista;
                gbLog.Height = 244;
                gbLog.Location = localizacaoGB;
            }
        }

        public void popularLog(string tipo, string mensagem)
        /*Método que quando é chamado (Recebendo duas Strings), preenche a lista lbLog e seleciona o item que foi preenchido*/
        {
            lbLog.Items.Add(System.DateTime.Now + " - " + tipo + ": " + mensagem);
            lbLog.SelectedIndex = Convert.ToInt32(lbLog.Items.Count - 1);
        }

        private void rbHabilitado_CheckedChanged(object sender, EventArgs e)
        {
            /*
                Valida se o Option referente ao WIFI está habilitado ou não
                Se estiver habilitado deixa as outras configurações abertas
                para edição
             */
            if (rbHabilitado.Checked)
            {
                gbModoWifi.Enabled = true;
                gbIdRede.Enabled = true;
                gbUsarWep.Enabled = true;
            }
            /*Se não, os componentes de entrada de dados permanecerão desabilidados*/
            else
            {
                gbModoWifi.Enabled = false;
                gbIdRede.Enabled = false;
                gbUsarWep.Enabled = false;
                gbChaveWep.Enabled = false;
            }
        }

        private void rbAdHoc_CheckedChanged(object sender, EventArgs e)
        {
            /*
                Valida se o Option referente ao ADHOC está habilitado ou não
                Se estiver habilitado deixa as outras configurações abertas
                para edição
             */
            if (rbAdHoc.Checked)
            {
                cbCanal.Enabled = true;
            }
            /*Se não, os componentes de entrada de dados permanecerão desabilidados*/
            else
            {
                cbCanal.Enabled = false;
            }
        }

        private void rbWepHab_CheckedChanged(object sender, EventArgs e)
        {
            /*
                 Valida se o Option referente ao WEP está habilitado ou não
                 Se estiver habilitado deixa as outras configurações abertas
                 para edição
             */
            if (rbWepHab.Checked)
            {
                gbChaveWep.Enabled = true;
            }
            /*Se não, os componentes de entrada de dados permanecerão desabilidados*/
            else
            {
                gbChaveWep.Enabled = false;
            }
        }

        private void btnReceberDinamico_Click(object sender, EventArgs e)
        {
            sc501ger.GetTermConectados(1, ref lista_terminais);
            //Usar as funções da DLL para receber informações 
            if(validarConectadoSelecionado())
            {
                try
                {

                    idTerminalSelecionado = lista_terminais.TabSock[lista_terminais_conectados.SelectedIndex];
                    //chamar a função bPedeParam para solicitar os parâmetros do equipamento
                    if (sc501ger.bPedeParam(idTerminalSelecionado))
                    {
                        popularLog("PROCESSO", "Parametros solicitadas com sucesso");//retornar se a função bPedeParam foi realizada com sucesso
                        Thread.Sleep(10);
                        //chamar a função bReceiveParam para receber os parâmetros do equipamento previamente solicitados
                        sc501ger.bReceiveParam(ref receberParametros);
                        popularLog("PROCESSO", "Parametros recebidos com sucesso");
                        
                        //preenche os radioBox conforme informações recebidas:  

                        if (receberParametros.ipdinamico == 0)
                        {
                            rbIpDinNao.PerformClick();
                        }
                        else
                        {
                            rbIpDinSim.PerformClick();
                        }
                        if (receberParametros.buscaserv == 0)
                        {
                            rbBuscServNao.PerformClick();
                        }
                        else
                        {
                            rbBuscServSim.PerformClick();
                        }
                    }
                }
                catch (Exception erro)
                {
                    popularLog("ERRO", "Configurações solicitadas com erro" + erro);
                }
            }
        }

        private void btnReceberSemFio_Click(object sender, EventArgs e)
        {
            sc501ger.GetTermConectados(1, ref lista_terminais);

            if (validarConectadoSelecionado())
            {
                try
                {
                    idTerminalSelecionado = lista_terminais.TabSock[lista_terminais_conectados.SelectedIndex];

                    //chamar a função bReceiveWlanConfig para solicitar os parâmetros do equipamento
                    if (sc501ger.bPedeWlanConfig(idTerminalSelecionado))
                    {
                        popularLog("PROCESSO", "Configurações Wlan solicitadas com sucesso");//retornar se a função bReceiveWlanConfig foi realizada com sucesso
                        Thread.Sleep(100);
                        sc501ger.bReceiveWlanConfig(ref receberWlanConfig);
                        
                        // Verifiicar se WIFI está HABILITADO ou DESABILITADO e preencher os respectivos componentes da tela  
                        popularLog("PROCESSO", "Configurações Wlan recebidas com sucesso");
                        if (receberWlanConfig.usewifi == 0)
                        {
                            rbDesabilitado.PerformClick();
                        }
                        else
                        {
                            rbHabilitado.PerformClick();
                            gbModoWifi.Enabled = true;
                            gbIdRede.Enabled = true;
                            gbUsarWep.Enabled = true;
                        }
                        
                        // Verificar o modo ADHOC ou INFRAESTRUTURA 
                        if (receberWlanConfig.mode == 0)
                        {
                            rbInfra.PerformClick();
                        }
                        else
                        {
                            rbAdHoc.PerformClick();
                            int canal = receberWlanConfig.channel;
                            cbCanal.SelectedItem = canal;
                        }
                        txtSsid.Text = ASCIIEncoding.Default.GetString(receberWlanConfig.ssid);
                        if (receberWlanConfig.wep == 0)
                        {
                            rbWepDesab.PerformClick();
                        }
                        else
                        {
                            rbWepHab.PerformClick();
                        }
                        txtChaveWep.Text = ASCIIEncoding.Default.GetString(receberWlanConfig.wepkey);
                    }
                }
                catch (Exception erro)
                {
                    popularLog("ERRO", "Configurações solicitadas com erro" + erro);
                }
            }
        }

        private void btnEnviarDinamico_Click(object sender, EventArgs e)
        {

            sc501ger.GetTermConectados(1, ref lista_terminais);
            idTerminalSelecionado = lista_terminais.TabSock[lista_terminais_conectados.SelectedIndex];

            //chamar a função bMandaParam para solicitar os parâmetros do equipamento
            if (validarConectadoSelecionado())
            {
                try
                {

                    //------------------MANDAR PARAMETROS PARA TERMINAL------------------//

                    //Verificar os objetos de entrada da tela e preencher o objeto 'mandarParametros', que é do tipo 'TCPARAMCONFIG'
                    mandarParametros.id = idTerminalSelecionado;
                    if (rbBuscServSim.Checked)
                    {
                        mandarParametros.buscaserv = 1;
                    }
                    else if (rbBuscServNao.Checked)
                    {
                        mandarParametros.buscaserv = 0;
                    }
                    if (rbIpDinSim.Checked)
                    {
                        mandarParametros.ipdinamico = 1;
                    }
                    else if (rbIpDinNao.Checked)
                    {
                        mandarParametros.ipdinamico = 0;
                    }
                    sc501ger.bMandaParam(ref mandarParametros);
                    popularLog("Processo", "Parâmetros enviados com sucesso");//caso não tenha dados nenhum erro, colocar na log que o processo foi realizado com sucesso
                    sc501ger.bSendRestartSoft(idTerminalSelecionado);//reiniciar o terminal que recebeu os dados
                    popularLog("Processo", "Reiniciando terminal de consulta");
                }
                catch (Exception erro)
                {
                    popularLog("ERRO", Convert.ToString(erro));

                }
            }
        }

        private void btnEnviarSemFio_Click(object sender, EventArgs e)
        {

            /*
                    NA EXECUÇÃO DO PROGRAMA, ESTE BOTÃO ESTÁ SETADO COMO INVISIVEL
             
             *      A função "sc501ger.bMandaWlanConfig(ref mandarWlanConfig)" está em desenvolvimento
             * 
             * 
             * 
             */



            sc501ger.GetTermConectados(1, ref lista_terminais);

            //chamar a função bMandaWlanConfig para solicitar os parâmetros do equipamento
            //através de uma estrutura do tipo TCWLANCONFIG
            if (validarConectadoSelecionado())
            {
                try
                {
                    //------------------MANDAR CONFIGURAÇÕES EXTRAS PARA TERMINAL------------------//
                    idTerminalSelecionado = lista_terminais.TabSock[lista_terminais_conectados.SelectedIndex];
                    if (rbHabilitado.Checked)
                    {
                        mandarWlanConfig.usewifi = 1;
                    }
                    if (rbDesabilitado.Checked)
                    {
                        mandarWlanConfig.usewifi = 0;
                    }
                    
                    
                    mandarWlanConfig.ID = idTerminalSelecionado;
                    byte[] ssid, chaveWep;
                    byte[] ssidConteudo, chaveWepConteudo = new byte[22];
                    
                    ssid = ASCIIEncoding.Default.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");
                    ssidConteudo = ASCIIEncoding.Default.GetBytes(txtSsid.Text);

                    /* 
                      O método é chamado passando:
                      uma referencia de um vetor de bytes não formatado, mas com as informações corretas, Exemplo:'ssidConteudo',
                      uma referencia de um outro vetor de bytes que receberá o conteudo formatado pelo 'método PreencherByte', Exemplo:'ssid';
                     */
                    preencherByte(ref ssidConteudo, ref ssid, 22);
                    mandarWlanConfig.ssid = ssid;
                    if (rbWepHab.Checked)
                    {
                        mandarWlanConfig.wep = 1;
                        chaveWep = ASCIIEncoding.Default.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");  //preenche o vetor de bytes com "Zeros" para ocupar o tamanho 
                        chaveWepConteudo = ASCIIEncoding.Default.GetBytes(txtChaveWep.Text);                        //preenche o vetor com as informações recebidas da caixa de texto
                        preencherByte(ref chaveWepConteudo, ref chaveWep, 22);
                        mandarWlanConfig.wepkey = chaveWep;
                    }
                    else
                    {
                        mandarWlanConfig.wep = 0;
                        chaveWep = ASCIIEncoding.Default.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");
                        mandarWlanConfig.wepkey = chaveWep;
                    }

                    if (rbInfra.Checked)
                    {
                        mandarWlanConfig.mode = 0;
                    }
                    else if (rbAdHoc.Checked)
                    {
                        mandarWlanConfig.mode = 1;
                        mandarWlanConfig.channel = Convert.ToByte(cbCanal.SelectedItem);
                    }
                    sc501ger.bMandaWlanConfig(ref mandarWlanConfig);
                    sc501ger.bSendRestartSoft(idTerminalSelecionado);
                }
                catch (Exception erro)
                {
                    popularLog("ERRO! ", Convert.ToString(erro));
                }
            }
        }

        private void btnProcurar_Click(object sender, EventArgs e)
        {
            //abre um componete OpenFile para procurar uma imagem no computador para enviá-la ao terminal
            
            string tamanhoImagem, altura, largura;
            string mensagemParaLog;
            ofAbrirImagem.FileName = "";
            if (ofAbrirImagem.ShowDialog() == DialogResult.OK)//caso o botão OK seja acionado, segue as ações abaixo
            {
                mensagemParaLog = "Abrindo Imagem: " + ofAbrirImagem.FileName;
                popularLog("Processo", mensagemParaLog);                        //manda para a log "Abrindo Imagem" + caminho do arquivo
                pbPrevia.ImageLocation = ofAbrirImagem.FileName;                //Abre a imagem em uma PictureBox
                pbPrevia.Load(ofAbrirImagem.FileName);
                altura = Convert.ToString(pbPrevia.Image.Height);
                largura = Convert.ToString(pbPrevia.Image.Width);               //Coloca na caixa de texto dimensões as dimensões do arquivo
                tamanhoImagem = largura + "X" + altura + " Pixels";


                txtCaminho.Text = Convert.ToString(ofAbrirImagem.FileName);     //Coloca o caminho da imagem na sua caixa de texto
                txtDimensoes.Text = tamanhoImagem;

                lblNomeImagem.Visible = true;
                lblNomeImagem.Text = System.IO.Path.GetFileName(ofAbrirImagem.FileName);
                btnAdicionarImagem.Enabled = true;
            }
        }

        private void btnLimparImagem_Click(object sender, EventArgs e)
        {
            ofAbrirImagem.FileName = "";                        //Limpar a imagem carregada da picture box
            pbPrevia.ImageLocation = ofAbrirImagem.FileName;
            lblNomeImagem.Visible = false;
            lblNomeImagem.Text = "Nome da imagem";
            btnAdicionarImagem.Enabled = false;
        }

        private void btnEnviarAoTerminal_Click(object sender, EventArgs e)
        {
            sc501ger.GetTermConectados(1, ref lista_terminais);

            //Envia imagem ao terminal selecionado
            if (validarConectadoSelecionado())
            {
                try
                {
                    idTerminalSelecionado = lista_terminais.TabSock[lista_terminais_conectados.SelectedIndex];  //Obter ID do terminal selecionado
                    int quantidadeDeCaracteres, repeticao, pausa;
                    repeticao = 1;
                    pausa = 0;
                    string caminho;
                    caminho = ofAbrirImagem.FileName;           //pegar o caminho + nome do arquivo
                    quantidadeDeCaracteres = caminho.Count();   //Contar caracteres do caminho 

                    
                    if (txtRepeticoes.Text != "")//Se o usuário entrar com quantidade de repetições, converter para inteiro 
                    {
                        repeticao = Convert.ToInt32(txtRepeticoes.Text);
                    }
                    if (txtPausa.Text != "")//Se o usuário entrar com pausa, converter para inteiro 
                    {
                        pausa = Convert.ToInt32(txtPausa.Text);
                    }

                    byte[] nomeArquivoEnviado = new byte[quantidadeDeCaracteres]; //criar um vetor de bytes do tamanho da quantidade de caracteres do caminho
                    nomeArquivoEnviado = System.Text.Encoding.ASCII.GetBytes(caminho);//carregar o caminho no vetor de bytes
                    /*
                        Mandar imagem para o terminal com as informações: 
                        idTerminal(Socket), caminho do arquivo, largura do display, altura do display, indice¹, qtdRepeticoes, pausaEntreImagens, redimensionar² 
                            ¹Indice= numero de sequencia de 1 a 4 para exibição da imagem ou 0 para exibição imediata
                            ²Redimensionar=0 para sim
                     */

                    sc501ger.bSendImageFromFile(idTerminalSelecionado, nomeArquivoEnviado, 128, 64, 0, repeticao, pausa, 0);
                    
                    //function bSendImageFromFile(ID: Integer; var filename : byte; width, height, index, loop, pause, resize : integer)
                }
                catch (Exception erro)
                {
                    popularLog("ERRO", Convert.ToString(erro));
                }
            }
        }

        private void btnSubirImagem_Click(object sender, EventArgs e)
        {
            /*Subir imagem de posição na lista de imagens*/
            int contarItens = lbImagens.Items.Count;
            int index = lbImagens.SelectedIndex;    //Numero de seleção na lista (0,1,2,3)
            int movimentarIndice;

            //a fila de imagens é um vetor publico de 4 posições. cada posição representa o indice do arquivo e armazena o número de cada imagem
            object auxiliar;
            if (contarItens > 1)
            {
                if (lbImagens.SelectedIndex.Equals(index)) //Se o item estiver com o index igual ao numero da lista, significa que ele está na posição inicial
                {
                    if (index > 0)
                    {
                        //Lista física - Implementada em memória
                        movimentarIndice = filaDeImagens[index - 1];        //Variável que armazena a posição anterior do vetor
                        filaDeImagens[index - 1] = filaDeImagens[index];    //posição anterior fica igual à posição atual, ou seja, o item sobe na lista
                        filaDeImagens[index] = movimentarIndice;            //posição "Atual" recebe o elemento anterior, que estava armazenado em 'movimentarIndice'
                        
                        //Lista lógica, vista pelo usuário. Implementada na lbImagens
                        auxiliar = lbImagens.Items[index];                  
                        lbImagens.Items[index] = lbImagens.Items[index - 1];
                        lbImagens.Items[index - 1] = auxiliar;
                        lbImagens.SelectedIndex = Convert.ToInt32(index - 1);
                    }
                }
            }

        }

        private void btnDescerImagem_Click(object sender, EventArgs e)
        {
            /*Descer imagem de posição na lista de imagens*/
            int index = lbImagens.SelectedIndex;
            int contarItens = lbImagens.Items.Count;
            int movimentarIndice;

            object auxiliar;

            if (contarItens > 1)
            {
                if (lbImagens.SelectedIndex.Equals(index))
                {
                    if (index < contarItens - 1)
                    {
                        //Lista física=implementada em vetor
                        movimentarIndice = filaDeImagens[index + 1];
                        filaDeImagens[index + 1] = filaDeImagens[index];
                        filaDeImagens[index] = movimentarIndice;
                        //lista lógica=implementada na lbImagens
                        auxiliar = lbImagens.Items[index];
                        lbImagens.Items[index] = lbImagens.Items[index + 1];
                        lbImagens.Items[index + 1] = auxiliar;
                        lbImagens.SelectedIndex = Convert.ToInt32(index + 1);
                    }
                }
            }
        }

        private void btnAdicionarImagem_Click(object sender, EventArgs e)
        {
            /*Adiciona uma imagem carregada na lista de no máximo 4 itens*/
            btnEnviarLista.Enabled = true;
            int indiceDaLista = lbImagens.Items.Count;

            if (indiceDaLista < 4)
            {
                filaDeImagens[indiceDaLista] = indiceDaLista;
                lbImagens.Items.Add(txtCaminho.Text);
                lbImagens.SelectedIndex = Convert.ToInt32(indiceDaLista);
            }
            else
            {
                popularLog("Aviso: ", "Sua lista não pode ter acima de 4 imagens");
            }
            btnAdicionar.Enabled = true;
        }

        private void btnEnviarLista_Click(object sender, EventArgs e)
        {
            /*Envia a lista de no máximo 4 itens carregada em lbImagens para o equipamento*/
            int quantidadeDeCaracteres;
            int itensLista = lbImagens.Items.Count;
            sc501ger.GetTermConectados(1, ref lista_terminais);

            if (validarConectadoSelecionado())
            {
                try
                {
                    /*Carrega os arquivos na memória do TC*/ 

                    string caminho;
                    idTerminalSelecionado = lista_terminais.TabSock[lista_terminais_conectados.SelectedIndex];//Id do terminal selecionado, ou seja, que irá receber os arquivos

                    /*1º - Limpar lista de imagens existentes no equipamento*/
                    byte[] nomeArquivoEnviadoReset = new byte[30];
                        //Passar o ID do terminal que será resetado, um vetor de bytes vazio, 0, 0, index=255, 0, 0, 0
                        sc501ger.bSendImageFromFile(idTerminalSelecionado, nomeArquivoEnviadoReset, 0, 0, 255, 0, 0, 0);
                        Thread.Sleep(100);

                    /* 2º - Loop para enviar todas as imagens da lista para o terminal*/

                    for (int contador = 0; contador < itensLista; contador++)               //contar quantos itens serão carregados para repetir o envio
                    {
                        caminho = Convert.ToString(lbImagens.Items[contador]);              //Obter o caminho do arquivo através da lista em lbImagens
                        quantidadeDeCaracteres = caminho.Count();                           //Contar quantos caracteres tem o nome e o caminho do arquivo
                        byte[] nomeArquivoEnviado = new byte[quantidadeDeCaracteres];       //criar um vetor de bytes para armazenar o caminho do arquivo que será enviado 
                        nomeArquivoEnviado = System.Text.Encoding.ASCII.GetBytes(caminho);  //armazenar o caminho do arquivo que será enviado
                        Thread.Sleep(100);
                        //function bSendImageFromFile(ID;filename; width, height, index, loop, pause, resize)
                        sc501ger.bSendImageFromFile(idTerminalSelecionado, nomeArquivoEnviado, 128, 64, contador + 1, Convert.ToInt32(txtRepeticoes.Text) + 1, Convert.ToInt32(txtPausa.Text) + 1, 0);
                        Thread.Sleep(100);
                    }
                    /*OBS: NESTA CHAMADA, EXISTEM PARAMETROS NÃO INFORMADOS NO MANUAL ATUAL
                            function bSendImageFromFile(ID: Integer; var filename : byte; width, height, index, loop, pause, resize : integer)            
                                    Onde:
                                    loop: quantidade de vezes em que é exibida de forma seguida
                                    pause: tempo entre troca de imagens
                                    resize: se precisa redimencionar a imagem (manda 0)
                               function bSendImageFromFile(ID: Integer; var filename : byte; width, height, index, loop, pause, resize : integer)
                    */
                }
                catch (Exception erro)
                {
                    popularLog("ERRO", Convert.ToString(erro));
                }
            }
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {

            if (txtCodBar.Text == "")
            {
                //Se não preencher este campo não fazer nada
                popularLog("ERRO", "Favor preencha o código de barras");
            }
            else if (txtDescricaoProd.Text == "")
            {
                //Se não preencher este campo não fazer nada
                popularLog("ERRO", "Favor preencha a descrição do produto");
            }
            else if (txtPrecoProd.Text == "")
            {
                //Se não preencher este campo não fazer nada
                popularLog("ERRO", "Favor preencha o preço do produto");
            }
            else
            {
                //Se o usuário preencher todas as informações, carregar na lista
                dgProdutos.Rows.Add(txtCodBar.Text, txtDescricaoProd.Text, txtPrecoProd.Text);
                popularLog("PROCESSO", ("Produto " + txtDescricaoProd.Text + " incluso com sucesso"));
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {


            acumulo++;


            byte[] bufRecebeu = new byte[30];
            int[] idTerminalEnviou = new int[5];
            int[] portaSerial = new int[5];
            int[] nrBarramento = new int[5];


            if (sc501ger.bReceiveBarcode(idTerminalEnviou, portaSerial, bufRecebeu, nrBarramento))
            {
                /*Tratar código de barras recebido*/

                int qtdProdListados = (dgProdutos.Rows.Count - 1); 
                string codBarras = "";
                int correrNoBuffer = 1;
                int correrAuxiliar = correrNoBuffer - 1;

                codBarras = Encoding.ASCII.GetString(bufRecebeu);
                codBarras = codBarras.Replace("#", "");
                codBarras = codBarras.Replace("\0", "");
                popularLog("Código de Barras Lido", codBarras);

                //percorrer a lista de produtos para "PROCURAR" o código recebido

                string valorCelula = "";
                string nomeProdutoTabela = "";
                string valorProdutoTabela = "";
                for (int contador = 0; contador < qtdProdListados; contador++)//Percorrer a lista de produtos para verificar se o item pesquisado está cadastrado
                {
                    //dataGridView1.Rows[1].Cells[0].Value = "new value"

                    valorCelula = Convert.ToString(dgProdutos.Rows[contador].Cells[0].Value);
                    if (valorCelula.Equals(codBarras))
                    {
                        nomeProdutoTabela = Convert.ToString(dgProdutos.Rows[contador].Cells[1].Value);
                        valorProdutoTabela = Convert.ToString(txtMoeda.Text + dgProdutos.Rows[contador].Cells[2].Value);
                    }
                }
                byte[] nomeProduto = new byte[5];
                byte[] precoProduto = new byte[5];

                //mandar o produto e o preço para o terminal que enviou o código de barras ou retornar mensagem de produto não cadastrado
                //VERIFICAR SE A FONTE "PERSONALIZADA ESTÁ CONFIGURADA"
                if (nomeProdutoTabela != "")
                {
                    nomeProduto = Encoding.Default.GetBytes(nomeProdutoTabela);
                    precoProduto = Encoding.Default.GetBytes(valorProdutoTabela);
                    
                    if (checkBox1.Checked)
                    {
                 
                        string caminho = AppDomain.CurrentDomain.BaseDirectory + "\\tmp.bmp";
                     
                        int quantidadeDeCaracteres = caminho.Count();                       

                        byte[] nomeArquivoEnviado = new byte[quantidadeDeCaracteres]; //criar um vetor de bytes do tamanho da quantidade de caracteres do caminho
                        nomeArquivoEnviado = System.Text.Encoding.ASCII.GetBytes(caminho);

                        txtProdPerson.Text = 
                            Convert.ToString(nomeProdutoTabela) +
                            "\n" + 
                            Convert.ToString(valorProdutoTabela);
                        String txt = txtProdPerson.Text; 
                        //carregar text com nome e preço

                        //gerar imagem personalizada e mandar para o terminal               
                        //cria a imagem bitmap
                        Bitmap bmp = new Bitmap(1, 1);
                        
                        //O método FromImage cria um novo Graphics a partir da imagem definida
                        Graphics graphics = Graphics.FromImage(bmp);
                        // Cria um objeto Font para desenhar o texto da imagem
                        Font font = new Font(fontDialog1.Font.Name, fontDialog1.Font.Size);
                        // Instancia o objeto Bitmap imagem novamente com o tamanho correto para o texto e fonte
                        SizeF stringSize = graphics.MeasureString(txt, font);
                       
                        //bmp = new Bitmap(bmp, (int)stringSize.Width, (int)stringSize.Height);
                       bmp = new Bitmap(bmp, (int)128, (int)64);

                        //bmp = new Bitmap((int)128, (int)64, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        //bmp = new Bitmap((int)128, (int)64, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);


                        graphics = Graphics.FromImage(bmp);
                        // Aqui temos uma outra possibilidade
                        // bmp = new Bitmap(bmp, new Size((int)graphics.MeasureString(txt, font).Width, (int)graphics.MeasureString(txt, font).Height));
                        //Desenha o texto com o formato definido
                       
                        graphics.Clear(Color.White);
                        graphics.DrawString(txt, font, Brushes.Blue, 0, 0);
                        
                        font.Dispose();
                        graphics.Flush();
                        graphics.Dispose();

                        bmp.Save(@caminho, System.Drawing.Imaging.ImageFormat.Bmp);

                        sc501ger.bSendImageFromFile(idTerminalEnviou[0], nomeArquivoEnviado, 128, 64, 0, 1, 5, 0);
                    }
                    else
                    {
                        sc501ger.bSendProdPrice(idTerminalEnviou[0], nomeProduto, precoProduto);
                    }    
                    popularLog("Produto localizado", nomeProdutoTabela + " " + valorProdutoTabela);
                }
                else //Se o item não for encontrado, chamar a função da DLL bSendProdNotFound, que retorna no display "Produto não cadastrado"
                {
                    sc501ger.bSendProdNotFound(idTerminalEnviou[0]);
                    popularLog("Produto localizado", "Produto não encontrado");
                }
                

            }

        }

        private void btnSalvarLog_Click(object sender, EventArgs e)
        {
            //Função que salva a log exibida em lbLog para um arquivo *.Log
            
            string dia, mes, ano, hora, minuto, segundo, nomePadrao, nomeArquivo;
            
            //Pega o dia e adiciona 0 caso o dia seja com 1 número
            dia = Convert.ToString(System.DateTime.Today.Day);
            if (Convert.ToInt16(dia) < 10)
            {
                dia = "0" + dia;
            }

            //Pega o mes e adiciona 0 caso o dia seja com 1 número
            mes = Convert.ToString(System.DateTime.Today.Month);
            if (Convert.ToInt16(mes) < 10)
            {
                mes = "0" + mes;
            }

            //Pega o ano
            ano = Convert.ToString(System.DateTime.Today.Year);

            //Pega a hora e adiciona 0 caso o dia seja com 1 número
            hora = Convert.ToString(System.DateTime.Now.Hour);
            if (Convert.ToInt16(hora) == 0)
            {
                hora = "00";
            }
            if (Convert.ToInt16(hora) < 10)
            {
                hora = "0" + hora;
            }

            //Pega o minuto
            minuto = Convert.ToString(System.DateTime.Now.Minute);
            if (Convert.ToInt16(minuto) == 0)
            {
                minuto = "00";
            }
            if (Convert.ToInt16(minuto) < 10)
            {
                minuto = "0" + minuto;
            }

            //Pega os segundos
            segundo = Convert.ToString(System.DateTime.Now.Second);
            if (Convert.ToInt16(segundo) == 0)
            {
                segundo = "00";
            }
            if (Convert.ToInt16(segundo) < 10)
            {
                segundo = "0" + segundo;
            }

            //Define o nome padrão como LOGDDMMAAAA-HHMMSS
            nomePadrao = "Log " + dia + mes + ano + " - " + hora + minuto + segundo;
            int quantidadeLinhasLog = lbLog.Items.Count;

            //cria um filtro para o componente saveFile
            sfSalvarLog.Filter = "Arquivo de Log (*.log)|*.log";
            sfSalvarLog.FileName = nomePadrao;
            sfSalvarLog.DefaultExt = ".log";

            //Se o usuário clicar em salvar o arquivo
            if (sfSalvarLog.ShowDialog() == DialogResult.OK)
            {
                //Cria um arquivo no local que o usuário define
                nomeArquivo = Convert.ToString(sfSalvarLog.FileName);
                StreamWriter writer = new StreamWriter(nomeArquivo);

                //Escreve Linha a Linha do arquivo, conforme cada linha do lbLog
                for (int contadorLinha = 0; contadorLinha < quantidadeLinhasLog; contadorLinha++)
                {
                    writer.WriteLine(lbLog.Items[contadorLinha]);

                }
                //Fecha o escritor
                writer.Close();
            }
        }

        private void btnApagar_Click(object sender, EventArgs e)
        {
            //apagar o item selecionado da lista de produtos 

            try
            {
                dgProdutos.Rows.RemoveAt(this.dgProdutos.SelectedRows[0].Index);
            }
            catch (ArgumentOutOfRangeException)
            {
                /*
                    - Erro acontece quando o indice da linha que será apagada ser inválido
                    - A linha que será apagada recebe o indece de SELECTEDROWS (linha selecionada)
                    - Esse erro ocorre quando tentamos apagar quando não há nenhum item selecionado
                 
                 */
                popularLog("ERRO", "Você deve selecionar uma linha antes de apagar");
            }
            catch (Exception erro)
            {
                /*
                    Caso ocorra outro erro não previsto, informar em LOG
                 */
                popularLog("ERRO", Convert.ToString(erro));
            }
        }


        private void lbImagens_Click(object sender, EventArgs e)
        {

            try
            {
                if (lbImagens.Items.Count > 0)
                {
                    //Ao clicar em cada item da lista de imagens, o item será exibido na PictureBox Prévia, se houver itens na lista

                    string caminho = Convert.ToString(lbImagens.Items[lbImagens.SelectedIndex]);
                    string altura = Convert.ToString(pbPrevia.Image.Height);
                    string largura = Convert.ToString(pbPrevia.Image.Width);
                    string tamanhoImagem = largura + "X" + altura + " Pixels";

                    txtDimensoes.Text = tamanhoImagem;
                    txtCaminho.Text = Convert.ToString(caminho);
                    //pbPrevia.ImageLocation = ofAbrirImagem.FileName;
                    pbPrevia.Load(caminho);

                    lblNomeImagem.Visible = true;
                    lblNomeImagem.Text = System.IO.Path.GetFileName(ofAbrirImagem.FileName);
                    btnAdicionarImagem.Enabled = true;
                }
            }
            catch (Exception erroNIdentificado)
            {
                popularLog("Erro", Convert.ToString(erroNIdentificado));
            }

        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            /*Botão para importar um arquivo com os produtos, assim como o priceTab e carrega na lista de produtos*/
            try
            {

                if (ofImportarProdutos.ShowDialog() == DialogResult.OK)
                {
                    popularLog("Processo", "Importando produtos...");
                    System.IO.StreamReader arquivoLido = new System.IO.StreamReader(@ofImportarProdutos.FileName);
                    txtCaminhoTxt.Text = Convert.ToString(ofImportarProdutos.FileName);
                    long percorrerLinha = 0;
                    string linha;
                    string[] retornoLido = new string[10];

                    while ((linha = arquivoLido.ReadLine()) != null)
                    {   
                        // Ler o arquivo linha a linha e carregar na lista
                        System.Console.WriteLine(linha);
                        retornoLido = linha.Split('|');
                        dgProdutos.Rows.Add(retornoLido[0], retornoLido[1], retornoLido[2]);
                        percorrerLinha++;
                    }
                    popularLog("Processo", Convert.ToString(percorrerLinha) + " Importados com sucesso");
                }
            }
            catch (IndexOutOfRangeException)
            {
                popularLog("Erro", "Quantidade de produtos acima do permitido");
            }

            catch (FileNotFoundException)
            {
                popularLog("Erro", "Este arquivo não existe");
            }
            catch (Exception erroNIdentificado)
            {
                popularLog("Erro", Convert.ToString(erroNIdentificado));
            }
        }
        public bool validarConectadoSelecionado()
        {
            bool retorno = false;
            try
            {
                TTABTERM lista_terminais = new TTABTERM(); // Instancia uma 'struct' do tipo TTABTERM
                sc501ger.GetTermConectados(1, ref lista_terminais); //Obtem a lista de terminais conectados

                //-- Validações de Terminal conectado e/ou selecionado --//
                if (lista_terminais.NumSockConec < 1)
                {
                    popularLog("ERRO", "Não Há Nenhum Terminal Conectado");
                }
                else
                {
                    if (lista_terminais_conectados.SelectedIndex < 0)
                    {
                        popularLog("ERRO", "Não Há Nenhum Terminal selecionado");
                    }
                    else
                    {
                        retorno = true;
                    }
                }
            }
            catch (Exception erro)
            {
                popularLog("Erro", "Erro Ao Enviar Comando!" + erro);
            }
            return retorno;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
                try
                {
                    sc501ger.GetTermConectados(1, ref lista_terminais); //Obtem a lista de terminais conectados
                    idTerminalSelecionado = lista_terminais.TabSock[lista_terminais_conectados.SelectedIndex];  //Obter ID do terminal selecionado
                    if (validarConectadoSelecionado())
                    {
                         sc501ger.bSendRestartSoft(idTerminalSelecionado);
                    }
                }
                catch (Exception erro)
                {
                    popularLog("ERRO! ", Convert.ToString(erro));
                }
            }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            /*
              Recebe um valor decimal, que quando convertido, exibe a versão da DLL
              Utiliza dois digitos para versão e dois digitos para revisão. 
              Exemplo: "0201"=2.1 
             */

            try
            {
                int versaoDLL, qtdeDigitos;
                string versaoDLLHexa, subStringVersaoPrincipal, subStringBuild;
                    versaoDLL =sc501ger.dll_version();                         //Recebe da DLL decimal que representa versão da DLL 
                    versaoDLLHexa = Convert.ToString(versaoDLL, 16);
                    qtdeDigitos = versaoDLLHexa.Count();
                    if (qtdeDigitos.Equals(7))
                    {
                        versaoDLLHexa = ("0" + versaoDLLHexa);
                    }
                    subStringVersaoPrincipal = versaoDLLHexa.Substring(0, 2);
                    subStringBuild = versaoDLLHexa.Substring(2, 2);
                    versaoDLLHexa = (Convert.ToInt32(subStringVersaoPrincipal) + "." + Convert.ToInt32(subStringBuild));
                MessageBox.Show("DLL SC501GER Versão: " + versaoDLLHexa, "Gertec Brasil");
            }
            catch(Exception erro) 
            {
                popularLog("ERRO! ", Convert.ToString(erro));
            }
            
        }

        private void btnFormatar_Click(object sender, EventArgs e)
        {    
            int itensLista = lbImagens.Items.Count;
            sc501ger.GetTermConectados(1, ref lista_terminais);

            if (validarConectadoSelecionado())
            {
                try
                {

                    idTerminalSelecionado = lista_terminais.TabSock[lista_terminais_conectados.SelectedIndex];//Id do terminal selecionado, ou seja, que irá receber os arquivos
                    /*1º - Limpar lista de imagens existentes no equipamento*/
                    byte[] nomeArquivoEnviadoReset = new byte[30];
                    sc501ger.bSendImageFromFile(idTerminalSelecionado, nomeArquivoEnviadoReset, 0, 0, 255, 0, 0, 0);
                }
                catch(Exception erro)
                {
                    popularLog("ERRO! ", Convert.ToString(erro));
                }



                    }
        }

        private void btnLimparLog_Click(object sender, EventArgs e)
        {
            lbLog.Items.Clear();
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            try
            {
                if (lbImagens.SelectedIndex >= 0)
                {
                    lbImagens.Items.RemoveAt(Convert.ToInt32(lbImagens.SelectedIndex));
                    lbImagens.SelectedIndex = (lbImagens.SelectedIndex - 1);
                }
                else { }
            }
            catch (Exception erro)
            {
                popularLog("ERRO! ", Convert.ToString(erro));
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        
            DialogResult result; 
            
            if (checkBox1.Checked) {
                result = fontDialog1.ShowDialog();
                if (result == DialogResult.OK) {
                    Font font = fontDialog1.Font;
                    lblNomeDaFonte.Text = "Fonte: " + font.Name;
                    lblTamFonte.Text = "Tamanho: " +  Convert.ToString(Convert.ToInt32(font.Size));
                }
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {

        }

        private void txtCodBar_TextChanged(object sender, EventArgs e)
        {

        }
        }
}
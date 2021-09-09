int forIteratorLevelN = 0;        
int [] forIteratorLevel;
forIteratorLevel = new int[10];



/*
MÁQUINA DE ESTADOS

    Estado 2 -> CURVA 90 GRAUS              // encruzilhada preta dupla // fake curva
Linha: 832


    Estado 1 -> SEGUIDOR DE LINHA           // sensores cor branco      // antbug 90 graus          // usa estado 2
Linha: 935


    Estado 11 -> ENTREGA VÍTIMAS            // alinha resgate           // alinha área de salvamento                      
Linha: 1035


    Estado 9 -> LOCALIZA VÍTIMAS
Linha:1081


    Estado 10 -> BUSCAR A VÍTIMA
Linha: 1008


    Estado 8 -> PROCURA ÁREA DE RESGATE     // usa estados 9, 10 e 11
Linha: 1141


    Estado 7 -> IDENTIFICADOR DA RAMPA      // estado 1 e 8
Linha: 1246
    

    Estado 6 -> DESVIAR DE OBSTACULO  
Linha: 1296


    Estado 3 -> ENCRUZILHADA VERDE
Linha: 1481
*/

// Determina um numero inicial pra cada variavel dos dados dos sensores
string SensorCorEsquerda =  "nulo";
string SensorCorDireita =  "nulo";
string SensorCorPontaDireita =  "nulo";
string SensorCorPontaEsquerda =  "nulo";
string SensorCorMeio =  "nulo";
string SensorCorFrente = "nulo";

string Estado = "nulo";

double SensorDistanciaFrenteBaixo = 0;
double SensorDistanciaFrenteCima = 0;
double SensorDistanciaDireita = 0;

double SensorDistanciaFrenteCimaSemDiferença = 0;

bool EstadoSensortoque = false;

bool DistanciaVitimaMenor25 = false;

double R = 0;
double B = 0;
double G = 0;

double H = 0;
double S = 0;

// Determina um nome pra cada sensor utilizado
int IDSensorCorPontaDireita = 0;
int IDSensorCorDireita = 1;
int IDSensorCorMeio = 2;
int IDSensorCorEsquerda = 3;
int IDSensorCorPontaEsquerda = 4;
int IDSensorCorFrente = 5;

int IDSensorDistanciaFrenteCima = 0;
int IDSensorDistanciarDireita = 1;
int IDSensorDistanciaFrenteBaixo= 2;

int IDSensorToque = 0;

//OBS: InclinaçãoDoRobô é no sentido de cima para baixo (eixo x)
float InclinaçãoDoRobô = 0;

float DireçãoDoRobô = 0;

bool Maior180 = false;
float AnguloZero = 0;
float AnguloFinal = 0;

float Vermelho = 0;
float Verde = 0;
float Azul = 0;

float RGBSensorCorMeio = 0;

//Determina os dados padrões das velocidades, distancias e inclinações
int VelocidadeVitimaMenor25Frente = 100;
int VelocidadeVitimaMenor25Tras = -100;

int DistanciaObstaculo = 5;

int AnguloDobraAreaSalvamento = 45;

int VelocidadeCurva = 500;
int VelocidadeAjuste = 200; // Do WhileAlinhamento
int VelocidadeMaximaFrente = 300;
int VelocidadeMaximaTras = -300;

int VelocidadeBranco = 120;

int PretoPontaEsquerda = 0;
int PretoPontaDireita = 0;
int PretoDireita = 0;
int PretoEsquerda = 0;

int DistanciaAlinhamentoVerde = 15;

int DistanciaAlinhaArea = 80;

float QuantidadeGiro = 0;
int SentidoCurva = 0;

float GiroIdentificaçãoVitima = 0;

int VelocidadeObstaculo = 135;

// Reformula as funções utilizadas

// Retorna determinada cor vista pelo sensor estabelecido no comando
Func<int, string> PegaCor = (sensorCor) => bc.returnColor(sensorCor);

// Retorna determinada distancia vista pelo sensor estabelecido no comando
Func<int, double> PegaDistancia = (sensorDist) => bc.distance(sensorDist);

Func<int, bool> PegaEstadoToque = (sensorToc) => bc.touch(sensorToc);

// Retorna determinada angulação (cima e baixo) vista pelo sensor estabelecido no comando
Func<float> InclinaçãoHorizontal = () => bc.inclination();

// Retorna determinada angulação (esquerda e direita) vista pelo sensor estabelecido no comando
Func<float> Direção = () => bc.compass();

//Retorna o tempo em milissegundos decorrido desde o início da execução da rotina ou desde o último comando de zerar o temporizador; 
Func<int> Temporizador = () => bc.timer();
//Zera o tempo decorrido no temporizador e inicia uma nova contagem


Action ZerarTemporizador = () => {
    bc.resetTimer();
};

// Escreve na tela o que esta definido entre os parenteses, onde o primeiro espaço é a linha que sera escrito no console e o segundo espaço é o texto em si
Action<int, string> EscreverNaTela = (linha, texto) => {
    bc.printLCD(linha, texto);
};

// Comando de andar, cada espaço nos parenteses é a força do motor
Action<int, int> Andar = (forçaMotorDireito, forçaMotorEsquerdo) => {
    bc.onTF(forçaMotorDireito, forçaMotorEsquerdo);
};

// Comando de rotacionar por graus, onde o primeiro espaço nos parenteses é a força dos motores e a segunda é o grau que o robô ira rotacionar NO PROPRIO EIXO 
Action<int, float> Rotacionar = (forçaMotor, angulo) => {
    bc.onTFRot(forçaMotor, angulo);
};

Action<float, float> AndarPorRotaçoes = (forçaMotores, rotaçoes) => {
    bc.onTFRotations(forçaMotores, rotaçoes);
};

// Liga o led na cor estabelecida nos parenteses
Action<string> LigarLed = (cor) => {
    bc.turnLedOn(cor);
};

// Tempo que será realizado a ação (após andar) ou tempo de espera(após rotacionar)
Action<int> Esperar = (tempo) => {
    bc.wait (tempo);
};

// Move o atuador pra cima, o numero entre os parenteses é o tempo de duração da ação
Action<int> AtuadorCima = (atuadorCima) => {
  bc.actuatorUp(atuadorCima);
};

// Move o atuador pra baixo, o numero entre os parenteses é o tempo de duração da ação
Action<int> AtuadorBaixo = (atuadorBaixo) => {
   bc.actuatorDown(atuadorBaixo);
};

// Move o atuador, em graus, pra cima, o numero entre os parenteses é o tempo de duração da ação
Action<int> GirarAtuadorCima = (tempoAtuadorCima) => {
  bc.turnActuatorUp(tempoAtuadorCima);
};

// Move o atuador, em graus, pra baixo, o numero entre os parenteses é o tempo de duração da ação
Action<int> GirarAtuadorBaixo = (tempoAtuadorBaixo) => {
   bc.turnActuatorDown(tempoAtuadorBaixo);
};

// Para ambos os motores do robô
Action Parar = () => {
    bc.onTF(0, 0);
};




//Atualiza o todas as variaveis com os retornos dos sensors quando chamada no código
Action AtualizarVariáveis = () => {
    
    SensorCorPontaEsquerda = PegaCor(IDSensorCorPontaEsquerda);
    SensorCorEsquerda = PegaCor(IDSensorCorEsquerda);
    SensorCorMeio = PegaCor(IDSensorCorMeio);
    SensorCorDireita = PegaCor(IDSensorCorDireita);
    SensorCorPontaDireita = PegaCor(IDSensorCorPontaDireita);
    SensorCorFrente = PegaCor(IDSensorCorFrente);

    SensorDistanciaFrenteBaixo = PegaDistancia(IDSensorDistanciaFrenteBaixo);
    SensorDistanciaFrenteCima = PegaDistancia(IDSensorDistanciaFrenteCima);
    SensorDistanciaDireita = PegaDistancia(IDSensorDistanciarDireita);

    EstadoSensortoque = PegaEstadoToque(IDSensorToque);

    SensorDistanciaFrenteCimaSemDiferença = SensorDistanciaFrenteCima - 6;
    
    InclinaçãoDoRobô = InclinaçãoHorizontal();
    DireçãoDoRobô = Direção();
    
    Vermelho = bc.returnRed(IDSensorCorFrente);
    Verde = bc.returnGreen(IDSensorCorFrente);
    Azul = bc.returnBlue(IDSensorCorFrente);
    RGBSensorCorMeio = bc.returnRed(IDSensorCorMeio);

    H = 0;
    
};


Action<int> VelocidadeAtuador = (velocidadeatuador) => {
    bc.actuatorSpeed(velocidadeatuador);
};


Action <int, float> WhileAlinhamento = (AnguloDestino, Sentido) => {  

    //Alinhamento (de acordo com a angulação) do robo
    while(Convert.ToInt32(DireçãoDoRobô) != AnguloDestino){
        Rotacionar(VelocidadeAjuste, Sentido);

        LigarLed("AZUL");

        Parar();
        AtualizarVariáveis();

    }
};








/*
De acordo com testes realizados pela equipe, constantemente o robo não identificava uma curva de 90 graus e ficava rotacionando para esquerda e direita entre as duas linhas na curva
    Como forma de resolver esse problema foi desenvolvido uma ideia onde o robo contará quantas vezes ele identificou preto em cada sensor
        Essa ideia foi separa em duas partes:

        - Parte A: SensorCorBranco
        Onde toda vez que o robo se alinhava com a linha preta (todos os sensores de cor, com exeção do meio, identificavam branco) era zerado o contador de linhas pretas identificadas

        - Parte B: AntBugCurva90Graus
        Onde era identificado que o robo localizou 2 vezes em cada sensor de cor a linha preta (indicando que ele estava rotacionando para esquerda e direita na curva de 90 graus), com isso ele realizava uma ré para que pudesse se realinhar na linha preta
    */

// Parte A
Action SensoresCorBranco = () => {

    AtualizarVariáveis();

    //Se alinha com a linha preta
    if(SensorCorDireita == "BRANCO" && SensorCorEsquerda == "BRANCO" && SensorCorPontaDireita == "BRANCO" && SensorCorPontaEsquerda == "BRANCO"){
        LigarLed("BRANCO");
        Estado = "Andando";
        
        Andar(VelocidadeBranco, VelocidadeBranco);
        Esperar(1);
        Parar();

        // Zera as variaveis utilizadas para identificar o bug da curva de 90 graus
        PretoPontaEsquerda = 0;
        PretoPontaDireita = 0;
        PretoDireita = 0;
        PretoEsquerda = 0;

        AtualizarVariáveis();

    }
};



//Parte B
Action AntBugCurva90Graus = () => {

    //Identifica que travou na curva de 90 graus
    if (PretoPontaDireita + PretoDireita >= 1 && PretoPontaEsquerda + PretoEsquerda >= 1){
        LigarLed( "PRETO" );
        Estado = "Travou na curva de 90 graus";       
        
        Andar(VelocidadeMaximaTras, VelocidadeMaximaTras);
        Esperar(70);
        Parar();

        // Zera as variaveis utilizadas para identificar o bug da curva de 90 graus
        PretoPontaEsquerda = 0;
        PretoPontaDireita = 0;
        PretoDireita = 0;
        PretoEsquerda = 0;


    } 
};
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//





// Anda em uma determinada velocidade na area de salvamento
Action AndaAreaSalvamento = () => {
    Estado = "Procurando área de resgate";

    Andar(VelocidadeMaximaFrente, VelocidadeMaximaFrente);
    Esperar(1);
};



// Estado onde todos os sensores de cor centrais identificam preto
Action EncruzilhadaPretaDupla = () => {

    //Idenfica uma encruzilhada preta dupla
    if(SensorCorMeio == "PRETO" && SensorCorPontaDireita == "PRETO" && SensorCorPontaEsquerda == "PRETO"){
        LigarLed("VERMELHO");

        Andar(VelocidadeMaximaFrente, VelocidadeMaximaFrente);
        Esperar(250);

        Parar();

        AtualizarVariáveis();

    }
};



Action TratamentoEncruzilhadaVerde = () => {

    AnguloZero = DireçãoDoRobô;

    /* 
    Para identificação das encruzilhadas separamos a angulação do robo em A (0 à 180) e B (180 à 360)
        Logo se a angulação do robo estiver no intervalo de B, o angulo do robo será convertido para o intervalo de A
            Isso facilitará a identificação de quanto o robo precisará dobrar e quanto será o angulo de sua dobra
    */

    //Se o angulo estiver em B
    if (AnguloZero > 180){

        //muda o angulo para um equivalente no intervalo de A
        AnguloZero = 360 - AnguloZero;
        Maior180 = true;
    }

    //Se o angulo estiver em A
    else {
        Maior180 = false;
    }

    //Gira para determinado lado para a não identificação da linha preta na frente da encruzilhada verde
    Rotacionar(VelocidadeCurva, (35 * SentidoCurva));

    AtualizarVariáveis();

    //Dobra para um determinado lado até o sensor de cor do meio virado para baixo identificar preto
    while(SensorCorMeio != "PRETO" && SensorCorFrente != "PRETO"){

        AtualizarVariáveis();

        Rotacionar(VelocidadeCurva, (0.01f * SentidoCurva));

        LigarLed("VERMELHO");
    }

    Parar();
    AtualizarVariáveis();

    //Chega a conclusão de que o robo esta na angulação ideal
    AnguloFinal = DireçãoDoRobô;
        
    //Iguala (em relação a proporcionalidade) o angulo ideal com o angulo atual do robo --> OBS: Isso so ocorre se a angulação do robo estiver em B
    if(Maior180){
        AnguloFinal = 360 - AnguloFinal;
    }
        
    //Conclui a quantidade final girada (em graus) que o robo realizou durante a ação
    QuantidadeGiro = AnguloFinal - AnguloZero;   
};



Action Tratamento90Graus = () => {

    /* 
    Para identificação das curvas de 90 graus separamos a angulação do robo em A (0 à 180) e B (180 à 360)
        Logo se a angulação do robo estiver no intervalo de B, o angulo do robo será convertido para o intervalo de A
            Isso facilitará a identificação de quanto o robo precisará dobrar e quanto será o angulo de sua dobra
    */

    AtualizarVariáveis();
    AnguloZero = DireçãoDoRobô;

    //Se o angulo estiver em B
    if (AnguloZero > 180){

        AnguloZero = 360 - AnguloZero;
        Maior180 = true;
    }

    //Se o angulo estiver em A
    else {
        Maior180 = false;
    }

    Rotacionar(VelocidadeCurva, (85 * SentidoCurva));

    AtualizarVariáveis();
    AnguloFinal = DireçãoDoRobô;

    //Iguala (em relação a proporcionalidade) o angulo ideal com o angulo atual do robo --> OBS: Isso so ocorre se a angulação do robo estiver em B 
    if(Maior180){
        AnguloFinal = 360 - AnguloFinal;
    }
        
    //Conclui a quantidade final girada (em graus) que o robo realizou durante a ação
    QuantidadeGiro = AnguloFinal - AnguloZero;    
};



// Alinha o robô de acordo com a angulação dele mais proxima da angulação das areas de resgate
Action AlinhaResgate = () => {

    AtualizarVariáveis();

    // 315 graus
    if (DireçãoDoRobô >= 300 && DireçãoDoRobô < 315){
        WhileAlinhamento(315, 0.1f);
    }
    else if (DireçãoDoRobô > 315 && DireçãoDoRobô <= 330){
        WhileAlinhamento(315, -0.1f);
    }

    //45 graus
    else if (DireçãoDoRobô >= 30 && DireçãoDoRobô < 45){
        WhileAlinhamento(45, 0.1f);
    }
    else if (DireçãoDoRobô > 45 && DireçãoDoRobô <= 60){
        WhileAlinhamento(45, -0.1f);
    }

    //135 graus
    else if (DireçãoDoRobô >= 120 && DireçãoDoRobô < 135){
        WhileAlinhamento(135, 0.1f);
    }
    else if (DireçãoDoRobô > 135 && DireçãoDoRobô <= 150){
        WhileAlinhamento(135, -0.1f);
    }

    //225 graus
    if (DireçãoDoRobô >= 240 && DireçãoDoRobô < 225){
        WhileAlinhamento(225, 0.1f);
    }
    else if (DireçãoDoRobô > 225 && DireçãoDoRobô <= 210){
        WhileAlinhamento(225, -0.1f);
    }
};



//Identifica uma curva em um angulo discreto em uma encruzilhada verde
Action IdentificadorCurva = () => {
    
    //Identifica se a encruzilhada é em um circulo 
    if (QuantidadeGiro >= 57 && QuantidadeGiro <= 63){
            
        Andar(VelocidadeMaximaTras, VelocidadeMaximaTras);
        Esperar(175);
        Parar();
        }

    /*
    As curvas que o robo deverá realizar durante o percurso na arena ficam restritas a determinados angulo -> 0/360, 90, 180, 270
        De tal forma, quando for identificado uma curva reta, o anfulo atual do robo será comparado a essas 4 possibilidades (OBS: a margem de erro da angulação foi estabelecida em 15, tanto pra mais quanto pra menos)
            Assim, o robo será capaz de distinguir para qual direção girar e o quanto será necessario rotacionar para chegar a determinado angulo
    */

    if (QuantidadeGiro >= 80 && QuantidadeGiro <= 100){

        AtualizarVariáveis();

        //Se o angulo for 0/360
        if (DireçãoDoRobô >= 345 && DireçãoDoRobô < 360){
            WhileAlinhamento(0, 0.01f);
        }
        else if (DireçãoDoRobô > 0 && DireçãoDoRobô <= 15){
            WhileAlinhamento(0, -0.01f);
        }


        //Se o angulo for 90
        else if (DireçãoDoRobô >= 75 && DireçãoDoRobô < 90){
            WhileAlinhamento(90, 0.01f);
        }
        else if (DireçãoDoRobô > 90 && DireçãoDoRobô <= 105){
            WhileAlinhamento(90, -0.01f);
        }


        //Se o angulo for 180
        else if (DireçãoDoRobô >= 165 && DireçãoDoRobô < 180){
            WhileAlinhamento(180, 0.01f);
        }
        else if (DireçãoDoRobô > 180 && DireçãoDoRobô <= 195){
            WhileAlinhamento(180, -0.01f);
        }


        //Se o angulo for 270
        else if(DireçãoDoRobô >= 255 && DireçãoDoRobô < 270){
            WhileAlinhamento(270, 0.01f);
        }
        else if(DireçãoDoRobô > 270 && DireçãoDoRobô <= 285){
            WhileAlinhamento(270, -0.01f);
        }
    }
};



// Se alinha quando identifica uma curva de 90 graus para identificar se é uma intercecção
Action AlinhaFakeCurva = () => {
    
    AtualizarVariáveis();

    //Se o angulo for 0/360
    if (DireçãoDoRobô >= 350 && DireçãoDoRobô < 360){
        WhileAlinhamento(0, 0.001f);
    }
    else if (DireçãoDoRobô > 0 && DireçãoDoRobô <= 10){
        WhileAlinhamento(0, -0.001f);
    }


    //Se o angulo for 90
    else if (DireçãoDoRobô >= 80 && DireçãoDoRobô < 90){
        WhileAlinhamento(90, 0.001f);
    }
    else if (DireçãoDoRobô > 90 && DireçãoDoRobô <= 100){
        WhileAlinhamento(90, -0.001f);
    }


    //Se o angulo for 180
    else if (DireçãoDoRobô >= 170 && DireçãoDoRobô < 180){
        WhileAlinhamento(180, 0.001f);
    }
    else if (DireçãoDoRobô > 180 && DireçãoDoRobô <= 190){
        WhileAlinhamento(180, -0.001f);
    }


    //Se o angulo for 270
    else if(DireçãoDoRobô >= 260 && DireçãoDoRobô < 270){
        WhileAlinhamento(270, 0.001f);
    }
    else if(DireçãoDoRobô > 270 && DireçãoDoRobô <= 280){
        WhileAlinhamento(270, -0.001f);
    }


    // 315 graus
    if (DireçãoDoRobô >= 300 && DireçãoDoRobô < 315){
        WhileAlinhamento(315, 0.1f);
    }
    else if (DireçãoDoRobô > 315 && DireçãoDoRobô <= 330){
        WhileAlinhamento(315, -0.1f);
    }

    //45 graus
    else if (DireçãoDoRobô >= 30 && DireçãoDoRobô < 45){
        WhileAlinhamento(45, 0.1f);
    }
    else if (DireçãoDoRobô > 45 && DireçãoDoRobô <= 60){
        WhileAlinhamento(45, -0.1f);
    }

    //135 graus
    else if (DireçãoDoRobô >= 120 && DireçãoDoRobô < 135){
        WhileAlinhamento(135, 0.1f);
    }
    else if (DireçãoDoRobô > 135 && DireçãoDoRobô <= 150){
        WhileAlinhamento(135, -0.1f);
    }

    //225 graus
    if (DireçãoDoRobô >= 240 && DireçãoDoRobô < 225){
        WhileAlinhamento(225, 0.1f);
    }
    else if (DireçãoDoRobô > 225 && DireçãoDoRobô <= 210){
        WhileAlinhamento(225, -0.1f);
    }
    
};



// Se alinha em um angulo discreto
Action AlinhaCurva = () => {

 AtualizarVariáveis();

    //Se o angulo for 0/360
    if (DireçãoDoRobô >= 330 && DireçãoDoRobô < 360){
        WhileAlinhamento(0, 0.001f);
    }
    else if (DireçãoDoRobô > 0 && DireçãoDoRobô <= 30){
        WhileAlinhamento(0, -0.001f);
    }


    //Se o angulo for 90
    else if (DireçãoDoRobô >= 60 && DireçãoDoRobô < 90){
        WhileAlinhamento(90, 0.001f);
    }
    else if (DireçãoDoRobô > 90 && DireçãoDoRobô <= 120){
        WhileAlinhamento(90, -0.001f);
    }


    //Se o angulo for 180
    else if (DireçãoDoRobô >= 150 && DireçãoDoRobô < 180){
        WhileAlinhamento(180, 0.001f);
    }
    else if (DireçãoDoRobô > 180 && DireçãoDoRobô <= 210){
        WhileAlinhamento(180, -0.001f);
    }


    //Se o angulo for 270
    else if(DireçãoDoRobô >= 240 && DireçãoDoRobô < 270){
        WhileAlinhamento(270, 0.001f);
    }
    else if(DireçãoDoRobô > 270 && DireçãoDoRobô <= 300){
        WhileAlinhamento(270, -0.001f);
    }

};



// Dobra uma angulação determinada na area de salvamento
Action DobraAreaSalvamento = () => {
    Estado = "Parede encontrada";

    Rotacionar(VelocidadeCurva, AnguloDobraAreaSalvamento);
    AlinhaCurva();
};



Action VarreduraAreaSalvamento = () => {
    
    /*
        Para otimizar o metodo de resgatar a vitimas, chegamos a conclusão de que é essencial a coleta precisa das mesmas
            Para isso fizemos um sistema que através da distancia da vitima em relação a parede o robô será capaz de busca-lá com a maior precisão
                Quando o robô identificar a vitima ele irá girar até parar de ver a vitima
                Após fazer isso o robô irá voltar dividindo a angulação rodada por 2
                Dessa forma, ele "encaixará" a vitima no centro do atuador, ou seja, o robõ irá pegar a vitima com uma grande precisão
    */

    // Determina o campo de visão do robô para a identificação de vitimas
    while(SensorDistanciaFrenteBaixo > 300 || SensorDistanciaFrenteCimaSemDiferença >= (SensorDistanciaFrenteBaixo - 30) && SensorDistanciaFrenteCimaSemDiferença <= (SensorDistanciaFrenteBaixo + 5) || Vermelho >= 10 && Vermelho <= 13 ||
    SensorDistanciaFrenteCima > 900 && SensorDistanciaFrenteBaixo >= 122 && SensorDistanciaFrenteBaixo <= 126 || 
    SensorDistanciaFrenteCima > 900 && SensorDistanciaFrenteBaixo >= 211 && SensorDistanciaFrenteBaixo <= 215 ||
    SensorDistanciaFrenteCima > 900 && SensorDistanciaFrenteBaixo >= 186 && SensorDistanciaFrenteBaixo <= 192 ||
    SensorDistanciaFrenteCima > 900 && SensorDistanciaFrenteBaixo >= 275 && SensorDistanciaFrenteBaixo <= 283 
    ){
    
    // Atribui os dados encontrados pelos sensores nas determinadas variaives destes dados
    AtualizarVariáveis();
    
    // Escreve na tela o retorno dos sensores, onde na linha 1 são os sensores de cor, a linha 2 são os sensores de distancia, a linha 3 é o grau de inclunação em relação ao solo e o estado atual
    EscreverNaTela(1,"SCPD: " + SensorCorPontaDireita + " | SCD: " + SensorCorDireita + " | SCM: " + SensorCorMeio + " | SCE: " + SensorCorEsquerda + " | SCPE: " + SensorCorPontaEsquerda);
    EscreverNaTela(2,"SDFC_SD: " + SensorDistanciaFrenteCimaSemDiferença + " | SDFB: " + SensorDistanciaFrenteBaixo);
    EscreverNaTela(3, "RGB(red): " + Vermelho.ToString() + " | " + "Fazendo: " + Estado);

    Rotacionar(VelocidadeCurva, 0.01f);
       
    }
    
    AnguloZero = Direção();

    // Com as distancias dos sensores de distancia da frente, será feito a rotação até que o robô pare de identificar a vitima
    while((PegaDistancia(0) - 6.5f) >= (PegaDistancia(2) + 2)){
        
    LigarLed("VERMELHO");
            
    // Escreve na tela o retorno dos sensores, onde na linha 1 são os sensores de cor, a linha 2 são os sensores de distancia, a linha 3 é o grau de inclunação em relação ao solo e o estado atual
    EscreverNaTela(1,"SCPD: " + SensorCorPontaDireita + " | SCD: " + SensorCorDireita + " | SCM: " + SensorCorMeio + " | SCE: " + SensorCorEsquerda + " | SCPE: " + SensorCorPontaEsquerda);
    EscreverNaTela(2,"SDFC_SD: " + SensorDistanciaFrenteCimaSemDiferença + " | SDFB: " + SensorDistanciaFrenteBaixo);
    EscreverNaTela(3, "RGB(red): " + Vermelho.ToString() + " | " + "Fazendo: " + Estado);

    Rotacionar(VelocidadeCurva, 0.01f);

        if(bc.returnRed(IDSensorCorFrente) >= 10 && bc.returnRed(IDSensorCorFrente) <= 13){
            break;
        }
    }

    EscreverNaTela(3, "Angulação da posição da vitima: " + GiroIdentificaçãoVitima.ToString() + " | " + "Fazendo: " + Estado);

    // Calcula o quanto o robô precisa voltar para ficar em frente a vitima e pega-lá
    AnguloFinal = Direção();
        
    if (AnguloZero > 180 && AnguloFinal < 180){
        
        AnguloZero = AnguloZero - 180;
        AnguloFinal = AnguloFinal + 180;

    }
    
    QuantidadeGiro = AnguloFinal - AnguloZero; 

    // Com a angulação ideal do robô calculada ele irá rotacionar esse angulo e ficar alinhado com a vitima
    EscreverNaTela(3, "Angulos: " + AnguloZero.ToString() + " | " + AnguloFinal.ToString());

    Rotacionar(VelocidadeCurva, (-QuantidadeGiro / 2f));

};



// O rbô ficará centralizado em frente a area de resgate
Action AlinhaAreaSalvamento = () => {   
    
    while(SensorDistanciaFrenteCima > DistanciaAlinhaArea){
            AtualizarVariáveis();

            Andar(VelocidadeMaximaFrente, VelocidadeMaximaFrente);
            Esperar(1);
            Parar();
    }
    
    while(SensorDistanciaFrenteCima < DistanciaAlinhaArea){
            AtualizarVariáveis();
            
            Andar(VelocidadeMaximaTras, VelocidadeMaximaTras);
            Esperar(1);
            Parar();
    }  
};



// Converte RGB em HSV sem a luminosidade
Func<int, double> RGB2HSV = (Sensor) => {

    //converte o codigo de cor RGB para HSV através de uma regra de 3
    R = bc.returnRed(Sensor) / 100;
    B = bc.returnBlue(Sensor) / 100;
    G = bc.returnGreen(Sensor) / 100;

    //Calcula o H(Hue) do HSV como forma de identificar a faixa prateada da area de salvamento

    // Primeiro caso -> MAX = R && G >= B
    if(R > G && G >= B){ 
        H = 60 * (G - B)/(R - B) + 0;

    }

    // Segundo caso -> MAX = R && G < B
    if(R > B && G < B){
        H = 60 * (G - B)/(R - G) + 360;

    }

    // Terceiro caso -> MAX = G
    if(G > R && G > B){
        // MIN = B
        if (B < R){
            H = 60 * (B - R)/(G - B) + 120;

        }
        // MIN = R
        if (R <= B){
            H = 60 * (B - R)/(G - R) + 120;

        }     
    }

    // Quarto caso -> MAX = B
    if(B > R && B > G){
        // MIN = R
        if (R < G){
        H = 60 * (R - G)/(B - R) + 240;

        }
        // MIN = G
        if (G <= R){
            H = 60 * (R - G)/(B - G) + 240;

        }       
    }
    EscreverNaTela(3, H.ToString());
    return H;
};







// Quando os dois sensores de cor identificam preto do mesmo lado ele dobra 90 graus para determinado lado
Action Estado_2 = () => {       

    EncruzilhadaPretaDupla();
    AtualizarVariáveis();

    if (SensorCorEsquerda ==  "PRETO"  && SensorCorPontaEsquerda ==  "PRETO" ){
        // Estado 2,1 -> Curva de 90 graus na linha preta (ESQUERDA)
        LigarLed( "PRETO" );
        Estado = "Curva de 90° Esquerda"; 
        
        AndarPorRotaçoes(VelocidadeMaximaFrente, 6);
        Parar();     

        AtualizarVariáveis();

        AlinhaFakeCurva();
        AtualizarVariáveis();

        Parar();

 

        //Identifica que é uma curva preta para a esquerda
        ZerarTemporizador();
        while (SensorCorMeio != "PRETO" && SensorCorDireita != "PRETO" && SensorCorEsquerda != "PRETO" && Temporizador() < 600){
            AtualizarVariáveis();
            Rotacionar(VelocidadeCurva, 0.01f);
        }
        
        if(SensorCorMeio != "PRETO" && SensorCorDireita != "PRETO" && SensorCorEsquerda != "PRETO" ){
            Rotacionar(VelocidadeCurva, -12);
            AlinhaFakeCurva();

            LigarLed("VERMELHO");

            AndarPorRotaçoes(VelocidadeMaximaFrente, 6);

            AtualizarVariáveis();

            Parar();

            SentidoCurva = -1;
            Tratamento90Graus();
            IdentificadorCurva();

            AndarPorRotaçoes(VelocidadeMaximaTras, 9);
            Parar();
            
        }
        else{
            Parar();
        }
        AtualizarVariáveis();
    }
    if (SensorCorDireita ==  "PRETO"  && SensorCorPontaDireita ==  "PRETO" ) {      
        // Estado 2,2 -> Curva de 90 graus na linha preta (DIREITA)
        LigarLed( "PRETO" );
        Estado = "Curva de 90° Direita";       
        
        AndarPorRotaçoes(VelocidadeMaximaFrente, 6);
        Parar();
        
        AtualizarVariáveis();

        AlinhaFakeCurva();
        
        AtualizarVariáveis();

        Parar();     


        //Identifica que é uma curva preta para a direita
        ZerarTemporizador();
        while (SensorCorMeio != "PRETO" && SensorCorDireita != "PRETO" && SensorCorEsquerda != "PRETO" && Temporizador() < 600){
            AtualizarVariáveis();
            Rotacionar(VelocidadeCurva, 0.01f);
        }

        if(SensorCorMeio != "PRETO" && SensorCorDireita != "PRETO" && SensorCorEsquerda != "PRETO"){
            Rotacionar(VelocidadeCurva, -12);
            AlinhaFakeCurva();

            LigarLed("VERMELHO");

            AndarPorRotaçoes(VelocidadeMaximaFrente, 6);
            AtualizarVariáveis();

            Parar();

            SentidoCurva = 1;
            Tratamento90Graus();
            IdentificadorCurva();

            AndarPorRotaçoes(VelocidadeMaximaTras, 9);
            Parar();

        }else{
            Parar();
        }
        AtualizarVariáveis();
    }
    AtualizarVariáveis();    
};







// Dobra para determinado lado quando um sesnsor identifica preto
Action Estado_1 = () => {

    AtualizarVariáveis();

    if(SensorCorPontaDireita == "PRETO"){
        // Estado 1,5 -> Identificador de linha preta(PONTADIREITA)   
        LigarLed( "PRETO" );
         Estado = "Dobrar Ponta Direita";
        Parar();

        Rotacionar(VelocidadeCurva, 1);

        Estado_2();

        //Conclui que a linha preta foi identificada mais uma vez pelo sensor
        PretoPontaDireita = PretoPontaDireita + 1;

    }
    if (SensorCorPontaEsquerda == "PRETO") { 
        // Estado 1,4 -> Identificador de linha preta (PONTAESQUERDA) 
        LigarLed( "PRETO" );
        Estado = "Dobrar Ponta Esquerda"; 
        Parar();

        Rotacionar(VelocidadeCurva, -1);

        Estado_2();

        //Conclui que a linha preta foi identificada mais uma vez pelo sensor
        PretoPontaEsquerda = PretoPontaEsquerda + 1;

    }
    if (SensorCorEsquerda == "PRETO") { 
         // Estado 1,2 -> Identificador de linha preta (ESQUERDA) 
        LigarLed( "PRETO" );
        Estado = "Dobrar Esquerda";
        Parar();

        Rotacionar(VelocidadeCurva, -0.01f);

        Estado_2();

        //Conclui que a linha preta foi identificada mais uma vez pelo sensor
        PretoEsquerda = PretoEsquerda + 1;
      
    }
    if (SensorCorDireita == "PRETO") {
         // Estado 1,3 -> Identificador de linha preta (DIREITA) 
        LigarLed( "PRETO" );
        Estado = "Dobrar Direita";
        Parar();

        Rotacionar(VelocidadeCurva, 0.01f);

        Estado_2();

        //Conclui que a linha preta foi identificada mais uma vez pelo sensor
        PretoDireita = PretoDireita + 1;
    }

    SensoresCorBranco();

    AntBugCurva90Graus();

};







// Pega a vitima
Action Estado_10 = () => {
    
    //Gira o atuador (tanto a posição do braço quanto o angulo do atuador) em uma determinada angulação
    VelocidadeAtuador(150);
    AtuadorBaixo(560);
    GirarAtuadorCima(140);

    Andar(VelocidadeMaximaFrente, VelocidadeMaximaFrente);
    Esperar(1000);
    AtuadorCima(300);
    Parar();
        
    //Gira o atuador (tanto a posição do braço quanto o angulo do atuador) em uma determinada angulação
    AtuadorCima(230);
    GirarAtuadorBaixo(650);  // Proporção 150

    Rotacionar(VelocidadeCurva, 150);
};








// Entrega a vitima na area de resgate
Action Estado_11 = () => {
    
    Estado = "Vítima salva";                                    
                                    
    Parar();

    VelocidadeAtuador(150);

    //Gira o atuador (tanto a posição do braço quanto o angulo do atuador) em uma determinada angulação
    AtuadorBaixo(400);
    GirarAtuadorBaixo(500);
    
    Esperar(300);



    //Gira o atuador (tanto a posição do braço quanto o angulo do atuador) em uma determinada angulação
    AtuadorCima(400);
    GirarAtuadorCima(500);
    
    // Identifica a area de resgate
    while(Vermelho >= 10 && Vermelho <= 13 || SensorDistanciaFrenteCimaSemDiferença >= (SensorDistanciaFrenteBaixo + 5)){

        AtualizarVariáveis();

        EscreverNaTela(2,"SDFC_SD: " + SensorDistanciaFrenteCimaSemDiferença + " | SDFB: " + SensorDistanciaFrenteBaixo);
        Rotacionar(VelocidadeCurva, 0.1f);

    }   
    Rotacionar(VelocidadeCurva, 5);

    AlinhaResgate();
    AlinhaAreaSalvamento();

    Parar();
    Rotacionar(VelocidadeCurva, 10); 
};                                                                      







// Inicia a varredura das vitimas girando no seu proprio eixo na lateral da area de resgate 
Action Estado_9 = () => {

    VarreduraAreaSalvamento();

    //Estado 9.2 -> Identifica uma vitima em uma distancia menor que 25
    if(SensorDistanciaFrenteBaixo < 25){
        Estado = "Vítima perto encontrada";
        
        EscreverNaTela(2,"SDFC_SD: " + SensorDistanciaFrenteCimaSemDiferença + " | SDFB: " + SensorDistanciaFrenteBaixo);
                                
        Parar();

        AndarPorRotaçoes(VelocidadeVitimaMenor25Frente, Convert.ToSingle((SensorDistanciaFrenteBaixo + 10) / 2));
        AndarPorRotaçoes(VelocidadeVitimaMenor25Tras, Convert.ToSingle((SensorDistanciaFrenteBaixo + 10) / 2));

        Esperar(1000);

        AtualizarVariáveis();

        Parar();
        Rotacionar(VelocidadeCurva, 10);
     
        AtualizarVariáveis();
        
        DistanciaVitimaMenor25 = true;
    }

    //Estado 9.3 -> Identifica uma vitima em uma distancia maior ou igual a que 25
    else{
        Estado = "Vítima encontrada";

        AndarPorRotaçoes(VelocidadeMaximaFrente, Convert.ToSingle((SensorDistanciaFrenteBaixo - 35) / 2));
        Parar();
    }
    
    //Se o robo tiver identificado uma vitima à uma distancia menor que 25, ele reiniciará a varredura em busca de outras vitimas
    if (DistanciaVitimaMenor25 == true){
        
        AtualizarVariáveis();
        Esperar(400);

        VarreduraAreaSalvamento();

        AndarPorRotaçoes(VelocidadeMaximaFrente, Convert.ToSingle((SensorDistanciaFrenteBaixo - 30) / 2));
        Parar();

        DistanciaVitimaMenor25 = false;
    }

    AtualizarVariáveis();
    Estado = "Vitima resgatada";
};







// Chega no topo da rampa e inicia a varredura, se guiando pelas paredes, para localizar a area de resgate OBS: INCLUI OS ESTADOS 9, 10, 11
Action Estado_8 = () => {

    //Robo identifica a area de resgate 
    if(Vermelho >= 10 && Vermelho <= 12){

        Estado = "Área de resgate encontrada";

        Parar();

        AtualizarVariáveis();             
        AlinhaCurva();
        
        Rotacionar(VelocidadeCurva, 45);
        AlinhaAreaSalvamento();
        
        //Se posiciona no meio da area de resgate para iniciar varredura das vitimas
        AlinhaResgate();        
        AlinhaAreaSalvamento();

        Parar();
        Rotacionar(VelocidadeCurva, 10);       
        

        //Inicia a varredura e o resgate das vitimas
        while(true){

            AtualizarVariáveis();

            Estado_9 ();

            Estado_10 ();
                            
            while(true){

                // Atribui os dados encontrados pelos sensores nas determinadas variaives destes dados
                AtualizarVariáveis();
    
                // Escreve na tela o retorno dos sensores, onde na linha 1 são os sensores de cor, a linha 2 são os sensores de distancia, a linha 3 é o grau de inclunação em relação ao solo e o estado atual
                EscreverNaTela(1,"SCPD: " + SensorCorPontaDireita + " | SCD: " + SensorCorDireita + " | SCM: " + SensorCorMeio + " | SCE: " + SensorCorEsquerda + " | SCPE: " + SensorCorPontaEsquerda);
                EscreverNaTela(2,"SDFC: " + SensorDistanciaFrenteCima + " | SDFB: " + SensorDistanciaFrenteBaixo);
                EscreverNaTela(3, "RGB(red): " + Vermelho.ToString() + " | " + "Fazendo: " + Estado);

                //Dobra quando encontra a parede
                if(SensorDistanciaFrenteCima <= 40){

                    DobraAreaSalvamento();
                }
                                
                //Estado 11 -> Entrega as vitimas na area de resgate
                if(Vermelho >= 10 && Vermelho <= 12){                                  
                                    
                    Estado_11 ();

                    break;                                                      
                }
                else{
                    
                     AtualizarVariáveis();

                    if (SensorDistanciaFrenteBaixo <= 5 && SensorDistanciaFrenteCima > 5){

                        AtualizarVariáveis();
                        Rotacionar(VelocidadeMaximaFrente, 30);
                    }
                    
                    AndaAreaSalvamento();
                }            
            }
        }
    }

    AtualizarVariáveis();
    
    //Dobra quando encontra a parede
    if(SensorDistanciaFrenteCima < 25){
        DobraAreaSalvamento();      

    }else{

        AtualizarVariáveis();
        AndaAreaSalvamento();
    }
        
};







// Identifica a rampa para a area de salvamento
Action Estado_7 = () => {
    
    AtualizarVariáveis();

    //Identifica a rampa da area de salvamento 
    if(RGB2HSV(IDSensorCorMeio) >= 88 && RGB2HSV(IDSensorCorMeio) <= 91){
        
        AtualizarVariáveis();

        //Prepara para área de resgate
        Estado = "Área de salvamento";
        LigarLed("AMARELO");
        
        //Identifica cada elemento da area de resgate (NAS LATERAIS)
        while(true){

            // Atribui os dados encontrados pelos sensores nas determinadas variaives destes dados
            AtualizarVariáveis();
    
            // Escreve na tela o retorno dos sensores, onde na linha 1 são os sensores de cor, a linha 2 são os sensores de distancia, a linha 3 é o grau de inclunação em relação ao solo e o estado atual
            EscreverNaTela(1,"SCPD: " + SensorCorPontaDireita + " | SCD: " + SensorCorDireita + " | SCM: " + SensorCorMeio + " | SCE: " + SensorCorEsquerda + " | SCPE: " + SensorCorPontaEsquerda);
            EscreverNaTela(2,"SDFC: " + SensorDistanciaFrenteCima + " | SDFB: " + SensorDistanciaFrenteBaixo);
            EscreverNaTela(3, "RGB(red): " + Vermelho.ToString() + " | " + "Fazendo: " + Estado);
                
            Parar();

            //Estado 8 -> Identifica a area de resgate
            Estado_8 ();
               
            }                     
    }
};







// Desvia do obstaculo fazendo um formato de triângulo

/*
De acordo com testes realizados pela equipe, a melhor opção para desviar do obstaculo seria um triangulo
Primeiramente o robo se alinharia com o obstaculo utilizando o sensor de distancia lateral (o que evitaria uma grande variação no angulo inicial do estado, proviniente de curvas e encruzilhadas)
Ele ultrapassaria o obstaculo realizando uma curva de 56 graus
Para se alinhar novamente com a linha preta, o robo andaria para frente até identificar a faixa preta na frente do obstaculo com os sensores de cor determinados
O robo andaria por um determinado tempo para trás, identificando se o obstaculo estaria proximo nesse meio tempo com o sensor de toque traseiro
*/

Action Estado_6 = () => {

    AtualizarVariáveis();
    
    // Identifica o obstaculo
    if (SensorDistanciaFrenteBaixo <= DistanciaObstaculo && InclinaçãoDoRobô < 2 && SensorCorFrente != "PRETO"){

        LigarLed("VERMELHO");
        Estado = "Desviando do Obstáculo";
        
        AtualizarVariáveis();

        AlinhaCurva();  

        AndarPorRotaçoes(VelocidadeMaximaTras, 7);

        LigarLed("AMARELO");
        
        // Gira para a ESQUERDA para localizar a linha preta ao lado do obstaculo
        Parar();
        Rotacionar(VelocidadeCurva, -30);
        
        LigarLed("AZUL");
        
        Andar(VelocidadeMaximaFrente, VelocidadeMaximaFrente);
        Esperar(200);
        Parar();

        AtualizarVariáveis();

        ZerarTemporizador();

        // Anda por 2300 milisegundos enquando busca a linha preta do lado do obstaculo 
        while(SensorCorDireita != "PRETO" && Temporizador() < 1500){

            Andar(VelocidadeObstaculo,VelocidadeObstaculo);

            AtualizarVariáveis();
        }

        Parar();
        
        // Caso 1 -> encontrou a linha preta - se alinha com a linha lateral do obstaculo
        if(SensorCorDireita == "PRETO"){

            LigarLed("VERMELHO");
            
            Andar(VelocidadeMaximaFrente, VelocidadeMaximaFrente);
            Esperar(530);

            Rotacionar(VelocidadeCurva, -40);

            AtualizarVariáveis();

            AlinhaCurva();

            AtualizarVariáveis(); 
            ZerarTemporizador();
            
            while(!EstadoSensortoque && Temporizador() < 400){

                AtualizarVariáveis();
        
                Andar(-200, -200);
            }

        }
        // Caso 2 -> não identificou a linha preta 
        else{

            ZerarTemporizador();
            AtualizarVariáveis();

            while(SensorCorPontaDireita != "PRETO" && Temporizador() < 4500){

                Andar(-VelocidadeObstaculo,-VelocidadeObstaculo);

                AtualizarVariáveis();
            }
            
            Rotacionar(VelocidadeCurva, 25);

            AtualizarVariáveis();

            AlinhaCurva();

            // Gira para a DIREITA para localizar a linha preta ao lado do obstaculo
            Rotacionar(VelocidadeCurva, 25);

            AtualizarVariáveis();
            ZerarTemporizador();

            while(SensorCorEsquerda != "PRETO" && Temporizador() < 2600){

                Andar(VelocidadeObstaculo,VelocidadeObstaculo);

                AtualizarVariáveis();
                   
            }

            // Caso 1 -> encontrou a linha preta - se alinha com a linha lateral do obstaculo 
            if(SensorCorEsquerda == "PRETO"){

                LigarLed("VERMELHO");
            
                Andar(VelocidadeMaximaFrente, VelocidadeMaximaFrente);
                Esperar(550);
                Parar();
                Rotacionar(VelocidadeCurva, 40);

                AtualizarVariáveis();

                AlinhaCurva();

                AtualizarVariáveis();
                ZerarTemporizador();
                
                while(!EstadoSensortoque && Temporizador() < 400){

                AtualizarVariáveis();
        
                Andar(-200, -200);
                }

            }
            // Caso 3 -> não identificou nenhuma linha preta nas laterais no obstaculo - o robô desviará pela frente
            else{

                Parar();

                Rotacionar(VelocidadeCurva, -20);

                AtualizarVariáveis();

                AlinhaCurva();

                AtualizarVariáveis();

                Andar(VelocidadeMaximaFrente, VelocidadeMaximaFrente);
                Esperar(500);

                Rotacionar(VelocidadeCurva, -35);

                AtualizarVariáveis();
                ZerarTemporizador();

                while(SensorCorEsquerda != "PRETO" && Temporizador() < 4500){

                    Andar(VelocidadeObstaculo, VelocidadeObstaculo);

                    AtualizarVariáveis();
                
                }

                Andar(VelocidadeMaximaFrente, VelocidadeMaximaFrente);
                Esperar(475);

                Rotacionar(VelocidadeCurva, 25);

                AtualizarVariáveis();

                AlinhaCurva();

                AtualizarVariáveis();
                ZerarTemporizador();

                while(!EstadoSensortoque && Temporizador() < 1000){

                    AtualizarVariáveis();
        
                    Andar(-200, -200);
                }

            }
               
        }
    }
};







// Dobra 60 graus para determinado lado quando um sesnsor identifica verde
Action Estado_3 = () => {

    AtualizarVariáveis();

    //Verifica se não é apenas uma sujeira na pista
    if (RGB2HSV(IDSensorCorPontaDireita) >= 107 && RGB2HSV(IDSensorCorPontaDireita) <= 114 || 
        RGB2HSV(IDSensorCorDireita) >= 107 && RGB2HSV(IDSensorCorDireita) <= 114 || 
        RGB2HSV(IDSensorCorEsquerda) >= 107 && RGB2HSV(IDSensorCorEsquerda) <=  114 || 
        RGB2HSV(IDSensorCorPontaEsquerda) >= 107 && RGB2HSV(IDSensorCorPontaEsquerda) <= 114)
    {

        LigarLed("AMARELO");
        Esperar(200);

        Andar(VelocidadeMaximaFrente, VelocidadeMaximaFrente);
        Esperar(5);
        Parar();

        AtualizarVariáveis();

        //OBS: MESMA LOGICA DOS ESTADOS TratamentoEncruzilhadaVerde() e IdentificadorCurva()


        if (RGB2HSV(IDSensorCorEsquerda) >= 107 && RGB2HSV(IDSensorCorEsquerda) <=  114){
            // Estado 3,1 -> Identificador de encruzilhada verde (ESQUERDA)
            LigarLed("VERDE");
            Estado = "Curva Verde Esquerda";
        
            AndarPorRotaçoes(VelocidadeMaximaFrente, DistanciaAlinhamentoVerde);
            Parar();

            AtualizarVariáveis();
        
            EscreverNaTela(1,"SCPD: " + SensorCorPontaDireita + " | SCD: " + SensorCorDireita + " | SCM: " + SensorCorMeio + " | SCE: " + SensorCorEsquerda + " | SCPE: " + SensorCorPontaEsquerda);

            SentidoCurva = -1;
        
            TratamentoEncruzilhadaVerde();

            IdentificadorCurva();
            AtualizarVariáveis();
        }

        if(RGB2HSV(IDSensorCorDireita) >= 107 && RGB2HSV(IDSensorCorDireita) <= 114){
            // Estado 3,2 -> Identificador de encruzilhada verde(DIREITA)   
            LigarLed("VERDE");
            Estado = "Curva Verde Direita";
        
            AndarPorRotaçoes(VelocidadeMaximaFrente, DistanciaAlinhamentoVerde);
            Parar();

            AtualizarVariáveis();
        
            EscreverNaTela(1,"SCPD: " + SensorCorPontaDireita + " | SCD: " + SensorCorDireita + " | SCM: " + SensorCorMeio + " | SCE: " + SensorCorEsquerda + " | SCPE: " + SensorCorPontaEsquerda);

            SentidoCurva = 1;
        
            TratamentoEncruzilhadaVerde();
            IdentificadorCurva();

            AtualizarVariáveis();
        }

        if (RGB2HSV(IDSensorCorPontaEsquerda) >= 107 && RGB2HSV(IDSensorCorPontaEsquerda) <= 114){
            // Estado 3,3 -> Identificador de encruzilhada verde(PONTAESQUERDA) 
            LigarLed("VERDE");
            Estado = "Curva Verde Ponta Esquerda";
        
            AndarPorRotaçoes(VelocidadeMaximaFrente, DistanciaAlinhamentoVerde);
            Parar();

            AtualizarVariáveis();
        
            EscreverNaTela(1,"SCPD: " + SensorCorPontaDireita + " | SCD: " + SensorCorDireita + " | SCM: " + SensorCorMeio + " | SCE: " + SensorCorEsquerda + " | SCPE: " + SensorCorPontaEsquerda);

            SentidoCurva = -1;
        
            TratamentoEncruzilhadaVerde();
            IdentificadorCurva();

            AtualizarVariáveis();
        }

        if(RGB2HSV(IDSensorCorPontaDireita) >= 107 && RGB2HSV(IDSensorCorPontaDireita) <= 114){
            // Estado 3,4 -> Identificador de encruzilhada verde(PONTADIREITA)   
            LigarLed("VERDE");
            Estado = "Curva Verde Ponta Direita";
        
            AndarPorRotaçoes(VelocidadeMaximaFrente, DistanciaAlinhamentoVerde);
            Parar();

            AtualizarVariáveis();
        
            EscreverNaTela(1,"SCPD: " + SensorCorPontaDireita + " | SCD: " + SensorCorDireita + " | SCM: " + SensorCorMeio + " | SCE: " + SensorCorEsquerda + " | SCPE: " + SensorCorPontaEsquerda);

            SentidoCurva = 1;
        
            TratamentoEncruzilhadaVerde();
            IdentificadorCurva();
        
            AtualizarVariáveis();
        }
        AtualizarVariáveis();
    }
};



//Desenha o percurso do robô na arena
// bc.paint();

//Define a velocidade da movimentação do atuador como 150
VelocidadeAtuador(150);


// Gira o atuador para cima durante 4 segundos
AtuadorCima(700);



// Inicia o loop da programação
while (true){
    
    // Atribui os dados encontrados pelos sensores nas determinadas variaives destes dados
    AtualizarVariáveis();
    
    
    // Escreve na tela o retorno dos sensores, onde na linha 1 são os sensores de cor, a linha 2 são os sensores de distancia, a linha 3 é o grau de inclunação em relação ao solo e o estado atual
    EscreverNaTela(1,"SCPD: " + SensorCorPontaDireita + " | SCD: " + SensorCorDireita + " | SCM: " + SensorCorMeio + " | SCE: " + SensorCorEsquerda + " | SCPE: " + SensorCorPontaEsquerda);
    EscreverNaTela(2,"SDFC: " + SensorDistanciaFrenteCima + " | SDFB: " + SensorDistanciaFrenteBaixo);
    EscreverNaTela(3, "Inclinação: " + InclinaçãoHorizontal() + " | " + "Fazendo: " + Estado);


//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//


    //Inicia a maquina de estados
    
    //Estado 7 -> identificador da area de salvamento
    Estado_7(); 

    // Estado 6 -> desviar obstaculo
    Estado_6(); 

    //Estado 2 -> identificador de preto
    Estado_2();
    
    // Estado 3 -> identificador de encruzilhada verde
    Estado_3();  

    // Estado 1 -> identificador de branco
    Estado_1();   


}


    
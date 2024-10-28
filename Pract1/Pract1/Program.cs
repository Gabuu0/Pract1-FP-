namespace Pract1
{
    class MainClass
    {
        const int DELTA = 400;
        const int FILS = 14, COLS = 22;
        const int FINAL = 2000;             //Tiempo del mensaje de Fin del juego
        static Random rnd = new Random();   // aleatorios para movimiento del enemigo

        
        public static void Main(string[] args)
        {
            // Console.SetWindowSize(width, height); // para poner consola de tamaño COLSxFILS

            Console.CursorVisible = false; // ocultamos cursor en pantalla

            int jugCol = 20, jugFil = 2,    //Posición inicial Jugador
                eneCol = 2, eneFil = 7,     //Posición inicial Enemigo
                balaFil = -1, balaCol = 0,  //Posición inicial Bala
                bombaCol =0 , bombaFil= -1, //Posición inicial Bomba
                finPartida ; //0 continua el juego; 1 gana jugador; 2 gana enemigo; 3 abortar
           
            bool bombaActiva = false;       //Comprobación de si ya hay una bomba en pantalla

            Console.Write("Pulsa cualquier tecla para empezar");
            
            while (!Console.KeyAvailable) Thread.Sleep(DELTA);
            Console.Clear();


            finPartida = 0;
            while (finPartida == 0)
            {

                // recogida de INPUT DE USUARIO
                string dir = "";
                if (Console.KeyAvailable)
                { // si se detecta pulsación de tecla
                    // leemos input y transformamos a string
                    dir = (Console.ReadKey(true)).KeyChar.ToString();
                    while (Console.KeyAvailable) Console.ReadKey(true); // limpiamos buffer de entrada						
                }

                //Movimiento del Jugador
                if (dir == "a" && jugFil > 0) jugFil--;
                if (dir == "d" && jugFil < FILS - 1) jugFil++;
                if (dir == "w" && jugCol > 0) jugCol--;
                if (dir == "s" && jugCol < COLS - 1) jugCol++;

                //Disparo y movimiento de la Bala
                if (dir == "l" && balaFil < 0)
                {
                    balaFil = jugFil;
                    balaCol = jugCol;
                }
                MovimientoBala(ref balaFil, ref balaCol);
                Colisiones(jugFil, jugCol, ref balaFil, balaCol, ref bombaFil, bombaCol, eneFil, eneCol, ref finPartida, ref bombaActiva);

                //Comportamiento del Enemigo
                MovimientoEnemigo(ref eneFil, ref eneCol);

                //Comportamiento de la Bomba
                AparicionBomba(ref bombaFil, ref bombaCol, eneFil, eneCol, ref bombaActiva);
                MovimientoBomba(ref bombaFil, ref bombaCol, ref bombaActiva);
                Colisiones(jugFil, jugCol, ref balaFil, balaCol, ref bombaFil, bombaCol, eneFil, eneCol, ref finPartida, ref bombaActiva);


                // RENDERIZADO 

                Console.Clear();
                RenderPlayer(jugFil, jugCol);

                RenderBala(balaFil, balaCol);

                RenderEnemigo(eneFil, eneCol);

                RenderBomba(bombaFil, bombaCol, bombaActiva);
                
                // retardo
                Thread.Sleep(DELTA);
                


            }
            //Opciones de Fin de Partida
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            if (finPartida == 1)
            {
                Console.WriteLine("Has ganao");
            }
            else if (finPartida == 2)
            {
                Console.WriteLine("Has perdio");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(FINAL);
            Console.WriteLine("Pulsa cualquier botón para salir.");

            while (!Console.KeyAvailable) Thread.Sleep(DELTA);
        }
            // bucle ppal
            
        #region Movimiento de Entidades
        static void MovimientoBala(ref int balaFil, ref int balaCol)
        {
            balaCol--;
            if (balaCol < 0)
            {
                balaFil = -1;
            }

        }
       
        static void MovimientoEnemigo(ref int eneFil, ref int eneCol)
        {
            int randomEneFil = rnd.Next(eneFil - 1, eneFil + 2);
            int randomEneCol = rnd.Next(eneCol - 1, eneCol + 2);

            if ((randomEneFil > 0 && randomEneCol >= 0) && (randomEneFil < FILS - 1 && randomEneCol <= (COLS - 1) / 2))
            {
                eneFil = randomEneFil;
                eneCol = randomEneCol;
            }
        }
        static void AparicionBomba(ref int bombaFil, ref int bombaCol, int eneFil, int eneCol, ref bool bombaActiva)
        {
            if (bombaFil < 0 && bombaActiva == false)
            {
                bombaFil = eneFil;
                bombaCol = eneCol;
                bombaActiva = true;
            }
        }
        static void MovimientoBomba(ref int bombaFil, ref int bombaCol, ref bool bombaActiva)
        {
            bombaCol++;
            if (bombaCol > COLS - 1)
            {
                bombaFil = -1;
                bombaActiva = false;
            }
        }
        #endregion

        #region Renderizados
        static void RenderPlayer(int jugFil, int jugCol)
        {
            Console.SetCursorPosition(jugFil, jugCol);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("O");

        }
        static void RenderBala(int balaFil, int balaCol)
        {
            if (balaFil != -1)
            {

                Console.SetCursorPosition(balaFil, balaCol);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("^");

            }
        }
        static void RenderEnemigo(int eneFil, int eneCol)
        {
            Console.SetCursorPosition(eneFil - 1, eneCol);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("<=>");
        }
        static void RenderBomba(int bombaFil, int bombaCol, bool bombaActiva) 
        {
            if (bombaActiva && bombaFil != -1)
            {
                Console.SetCursorPosition(bombaFil, bombaCol);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("*");
            }
        }
        #endregion

        #region Colisiones
        static void Colisiones(int jugFil, int jugCol,
                               ref int balaFil, int balaCol,
                               ref int bombaFil, int bombaCol,
                               int eneFil, int eneCol,
                               ref int finPartida, ref bool bombaActiva)
        {
            if (balaCol == eneCol && (eneFil - balaFil <= 1 && eneFil - balaFil >= -1))                     //Jugador Gana         
            {
                finPartida = 1;
            }
            else if ((bombaCol == jugCol && bombaFil == jugFil) || (eneCol == jugCol && eneFil == jugFil))  //Jugador Pierde
            {
                finPartida = 2;
            }
            if (balaCol == bombaCol && balaFil == bombaFil)
            {
                balaFil = -1;
                bombaFil = -1;
                bombaActiva = false;
            }
        }
        #endregion


    }
}
    

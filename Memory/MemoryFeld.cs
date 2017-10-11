using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;


namespace Memory
{
    class MemoryFeld
    {
        #region Default
        //das Array für die Karten
        private MemoryKarte[] karten;

        //das Array für die Namen der Grafiken
        private string[] bilder = 
	    {"grafiken/apfel.bmp", "grafiken/birne.bmp", 
	    "grafiken/blume.bmp", "grafiken/blume2.bmp",
	    "grafiken/ente.bmp", "grafiken/fisch.bmp", 
	    "grafiken/fuchs.bmp", "grafiken/igel.bmp",
	    "grafiken/kaenguruh.bmp", "grafiken/katze.bmp", 
	    "grafiken/kuh.bmp", "grafiken/maus1.bmp",
	    "grafiken/maus2.bmp", "grafiken/maus3.bmp", 
	    "grafiken/melone.bmp", "grafiken/pilz.bmp",
	    "grafiken/ronny.bmp", "grafiken/schmetterling.bmp",
	    "grafiken/sonne.bmp", "grafiken/wolke.bmp", 
	    "grafiken/maus4.bmp"};

        //für die Punkte
        private int menschPunkte, computerPunkte;
        private Label menschPunkteLabel, computerPunkteLabel;

        //wie viele Karten sind aktuell umgedreht?
        private int umgedrehteKarten;

        //für das aktuell umdrehte Paar
        private MemoryKarte[] paar;

        //für den aktuellen Spieler
        private int spieler;

        //das "Gedächtnis" für den Computer
        //er speichert hier, wo das Gegenstück liegt
        private int[,] gemerkteKarten;

        //für das eigentliche Spielfeld
        private UniformGrid feld;

        //für den Timer
        DispatcherTimer timer, timer1;

        //Button für Schummel
        Button btnSchummel;

        //für die Spielstärke
        int spielstaerke;
        #endregion

        #region Aufgabe 2
        //der Konstruktor
        public MemoryFeld(UniformGrid feld)
        {
            //zum Zählen für die Bilder
            int count = 0;
            //das Array für die Karten erstellen, insgesamt 42 Stück
            karten = new MemoryKarte[42];
            //für das Paar
            paar = new MemoryKarte[2];
            //für das Gedächtnis
            //es speichert für jede Karte paarweise die Position im Spielfeld
            gemerkteKarten = new int[2, 21];
            //für das Mischen der Karten
            Random zufallszahl = new Random();
            //keiner hat zu Beginn einen Punkt
            menschPunkte = 0;
            computerPunkte = 0;
            //es ist keine Karte umgedreht
            umgedrehteKarten = 0;
            //der Mensch fängt an
            spieler = 0;
            //die Spielstärke setzen
            spielstaerke = 10;
            //das Spielfeld setzen
            this.feld = feld;
            //es gibt keine gemerkten Karten
            for (int aussen = 0; aussen < 2; aussen++)
                for (int innen = 0; innen < 21; innen++)
                    gemerkteKarten[aussen, innen] = -1;
            //das eigentliche Spielfeld erstellen
            for (int i = 0; i <= 41; i++)
            {
                //eine neue Karte erzeugen
                karten[i] = new MemoryKarte(bilder[count], count, this);
                //bei jeder zweiten Karte kommt auch ein neues Bild
                if ((i + 1) % 2 == 0)
                    count++;
            }
            //die Karten mischen
            for (int i = 0; i <= 41; i++)
            {
                int temp1;
                MemoryKarte temp2;
                //eine zufällige Zahl im Bereich 0 bis 41 erzeugen
                temp1 = zufallszahl.Next(42);
                //den alten Wert in Sicherheit bringen
                temp2 = karten[temp1];
                //die Werte tauschen
                karten[temp1] = karten[i];
                karten[i] = temp2;
            }
            //die Karten ins Spielfeld bringen
            for (int i = 0; i <= 41; i++)
            {
                //die Position der Karte setzen
                karten[i].SetBildPos(i);
                //die Karte hinzufügen
                feld.Children.Add(karten[i]);
            }
            //die Labels für die Punkte
            Label mensch = new Label();
            mensch.Content = "Mensch";
            feld.Children.Add(mensch);
            menschPunkteLabel = new Label();
            menschPunkteLabel.Content = 0;
            feld.Children.Add(menschPunkteLabel);

            Label computer = new Label();
            computer.Content = "Computer";
            feld.Children.Add(computer);
            computerPunkteLabel = new Label();
            computerPunkteLabel.Content = 0;
            feld.Children.Add(computerPunkteLabel);

            //Button für Schummel
            btnSchummel = new Button();
            btnSchummel.Content = "Schummel";
            btnSchummel.IsEnabled = true;
            btnSchummel.Click += new RoutedEventHandler(btnSchummel_Click);
            feld.Children.Add(btnSchummel);            

            //den Timer erzeugen
            //er wird alle zwei Sekunden ausgelöst
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(2000);
            timer.Tick += new EventHandler(timer_Tick);
        }
        //die Methode für Button Schummel
        private void btnSchummel_Click(object sender, RoutedEventArgs e)
        {
            //Jede Karte zeigen
            for (int i = 0; i < 42; i++)
            {
                if (karten[i].IsNochImSpiel() == true)
                {
                    karten[i].VorderseiteZeigen();
                }
            }
            //den Timer erzeugen
            //er wird alle 3 Sekunden ausgelöst
            timer1 = new System.Windows.Threading.DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(3000);
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();
        }
        //Timer für Schummel
        private void timer1_Tick(object sender, EventArgs e)
        {
            //den Timer anhalten 
            timer1.Stop();
            //die Karten zurückdrehen
            for (int i = 0; i < 42; i++)
            {
                if (karten[i].IsNochImSpiel() == true)
                {
                    karten[i].RueckseiteZeigen();
                }
            }
        }
        //die Methode wechselt den Spieler
        private void spielerWechseln()
        {
            //wenn der Mensch an der Reihe war, kommt jetzt der Computer
            if (spieler == 0)
            {
                spieler = 1;
                computerZug();
                //Button Schummel ausschalten
                btnSchummel.IsEnabled = false;
            }
            else
            {
                spieler = 0;
                //Button Schummel anschalten
                btnSchummel.IsEnabled = true;
            }
        }
        //die Methode dreht die Karten wieder auf die Rückseite 
        //beziehungsweise nimmt sie aus dem Spiel
        private void karteSchliessen()
        {
            bool raus = false;
            //ist es ein Paar?
            if (paar[0].GetBildID() == paar[1].GetBildID())
                raus = true;
            //wenn es ein Paar war, nehmen wir die Karten aus 
            //dem Spiel, sonst drehen wir sie nur wieder um
            paar[0].RueckseiteZeigen(raus);
            paar[1].RueckseiteZeigen(raus);
            //es ist keine Karte mehr geöffnet
            umgedrehteKarten = 0;
            //hat der Spieler kein Paar gefunden?
            if (raus == false)
                //dann wird der Spieler gewechselt
                spielerWechseln();
            //Wenn Mensch spielt weiter, dann schalten wir Button Schummel wieder an
            else
                if (spieler == 0)
                    btnSchummel.IsEnabled = true;
                //hat der Computer ein Paar gefunden?
                //dann ist er noch einmal an der Reihe
                else
                    if (spieler == 1)
                        computerZug();
        }
        #endregion

        //die Methode übernimmt die wesentliche Steuerung des Spiels
        //Sie wird beim Anklicken einer Karte ausgeführt
        public void KarteOeffnen(MemoryKarte karte)
        {
            //zum Zwischenspeichern der ID und der Position
            int kartenID, kartenPos;

            //die Karten zwischenspeichern
            paar[umgedrehteKarten] = karte;

            //die ID und die Position beschaffen
            kartenID = karte.GetBildID();
            kartenPos = karte.GetBildPos();

            //die Karte in das Gedächtnis des Computers eintragen,
            //aber nur dann, wenn es noch keinen Eintrag an der 
            //entsprechenden Stelle gibt
            if ((gemerkteKarten[0, kartenID] == -1))
                gemerkteKarten[0, kartenID] = kartenPos;
            else
                //wenn es schon einen Eintrag gibt und der nicht mit 
                //der aktuellen Position übereinstimmt, dann haben wir 
                //die zweite Karte gefunden
                //Sie wird in die zweite Dimension eingetragen
                if (gemerkteKarten[0, kartenID] != kartenPos)
                    gemerkteKarten[1, kartenID] = kartenPos;
            //umgedrehte Karten erhöhen
            umgedrehteKarten++;

            //sind 2 Karten umgedreht worden?
            if (umgedrehteKarten == 2)
            {
                //dann prüfen wir, ob es ein Paar ist
                paarPruefen(kartenID);
                //den Timer zum Umdrehen der Karten starten
                timer.Start();
            }

            //haben wir zusammen 21 Paare, dann ist das Spiel vorbei
            if (computerPunkte + menschPunkte == 21)
            {
                //Der Name des Gewinners
                string name;
                //Wie viele Paare der Gewinner gezogen hat
                int punkte;
                //Punkte vergleich
                if (menschPunkte > computerPunkte)
                {
                    name = "Mensch";
                    punkte = menschPunkte;
                }
                else
                {
                    name = "Computer";
                    punkte = computerPunkte;
                }
                //den Timer anhalten
                timer.Stop();
                MessageBox.Show("Das Spiel ist vorbei." + "\nDer Gewinner ist " + name + ".\nDer " + name + " hat " + punkte + " Paare gezogen.", "Ende", MessageBoxButton.OK, MessageBoxImage.Information);
                //Anwendung herunterfahren
                Application.Current.Shutdown();
            }
            
            //Wenn umgedrehte Karte ist 1, dann schalten wir Button Schummel aus
            if (umgedrehteKarten == 1)
            {
                btnSchummel.IsEnabled = false;
            }
        }

        //die Methode für den Timer
        private void timer_Tick(object sender, EventArgs e)
        {
            //den Timer anhalten 
            timer.Stop();
            //die Karten zurückdrehen
            karteSchliessen();
        }
        //die Methode prüft, ob ein Paar gefunden wurde
        private void paarPruefen(int kartenID)
        {
            if (paar[0].GetBildID() == paar[1].GetBildID())
            {
                //die Punkte setzen
                paarGefunden();
                //die Karten aus dem Gedächtnis löschen
                gemerkteKarten[0, kartenID] = -2;
                gemerkteKarten[1, kartenID] = -2;
            }
        }
        //die Methode setzt die Punkte, wenn ein Paar gefunden wurde
        private void paarGefunden()
        {
            //spielt gerade der Mensch?
            if (spieler == 0)
            {
                menschPunkte++;
                menschPunkteLabel.Content = menschPunkte.ToString();
            }
            else
            {
                computerPunkte++;
                computerPunkteLabel.Content = computerPunkte.ToString();
            }
        }
        //die Methode setzt die Computerzüge um
        private void computerZug()
        {
		    int kartenZaehler = 0;
		    int zufall = 0;
		    bool treffer = false;
            Random zufallszahl = new Random();
            
            //zur Steuerung über die Spielstärke
            if (zufallszahl.Next(spielstaerke) == 0) 
            {
		        //erst einmal nach einem Paar suchen
		        //dazu durchsuchen wir das Array gemerkteKarten, bis wir in beiden 
                //Dimensionen einen Wert für eine Karte finden
                    while ((kartenZaehler < 21) && (treffer == false))
                    {
                        //gibt es in beiden Dimensionen einen Wert größer oder gleich 0?
                        if ((gemerkteKarten[0, kartenZaehler] >= 0) && (gemerkteKarten[1, kartenZaehler] >= 0))
                        {
                            //dann haben wir ein Paar
                            treffer = true;
                            //die erste Karte umdrehen durch einen simulierten Klick 
                            //auf die Karte
                            //der simulierte Klick wird nicht mehr ausgeführt
                            //karten[gemerkteKarten[0, kartenZaehler]].RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            //die Vorderseite zeigen
                            karten[gemerkteKarten[0, kartenZaehler]].VorderseiteZeigen();
                            //und die Karte öffnen
                            KarteOeffnen(karten[gemerkteKarten[0, kartenZaehler]]);
                            //die zweite Karte auch
                            //karten[gemerkteKarten[1, kartenZaehler]].RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            //die Vorderseite zeigen
                            karten[gemerkteKarten[1, kartenZaehler]].VorderseiteZeigen();
                            //und die Karte öffnen
                            KarteOeffnen(karten[gemerkteKarten[1, kartenZaehler]]);                        
                        }
                        kartenZaehler++;
                    }
		    }
		    //wenn wir kein Paar gefunden haben, drehen wir zufällig zwei Karten um
		    if (treffer == false) 
            {
			    //solange eine Zufallszahl suchen, bis eine Karte gefunden wird,
                //die noch im Spiel ist
                do
                {
                    zufall = zufallszahl.Next(42);
                } while (karten[zufall].IsNochImSpiel() == false);
			    //die erste Karte umdrehen
			    //karten[zufall].RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //die Vorderseite zeigen
                karten[zufall].VorderseiteZeigen();
                //und die Karte öffnen
                KarteOeffnen(karten[zufall]);                        

			    //für die zweite Karte müssen wir außerdem prüfen, ob sie 
                //nicht gerade angezeigt wird
			    do 
                {
                    zufall = zufallszahl.Next(42);
			    } while ((karten[zufall].IsNochImSpiel() == false) || (karten[zufall].IsUmgedreht() == true));
			    //und die zweite Karte umdrehen
                //karten[zufall].RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //die Vorderseite zeigen
                karten[zufall].VorderseiteZeigen();
                //und die Karte öffnen
                KarteOeffnen(karten[zufall]);                        
		    }
        }
        //die Methode liefert, ob Züge des Menschen erlaubt sind
        //die Rückgabe ist false, wenn gerade der Computer zieht oder wenn 
        //schon zwei Karten umgedreht sind
        //sonst ist die Rückgabe true
        public bool ZugErlaubt()
        {
            bool erlaubt = true;
            //zieht der Computer?
            if (spieler == 1)
                erlaubt = false;
            //sind schon zwei Karten umdreht?
            if (umgedrehteKarten == 2)
                erlaubt = false;
            return erlaubt;
        }
    }
}

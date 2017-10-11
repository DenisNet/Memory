using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Memory
{
    //die Klasse für eine Karte des Memory-Spiels
    //Sie erbt von Button
    class MemoryKarte : Button
    {
        //die Felder
	    //eine eindeutige ID zur Identifizierung des Bildes
	    private int bildID;
	    //für die Vorder- und Rückseite
	    private Image bildVorne, bildHinten;

	    //wo liegt die Karte im Spielfeld?
	    private int bildPos;

	    //ist die Karte umgedreht?
	    private bool umgedreht;
	    //ist die Karte noch im Spiel?
	    private bool nochImSpiel;

        //für das Spielfeld für die Karte
        private MemoryFeld spiel;

    	//der Konstruktor
    	//er setzt die Größe, die Bilder und die Position
    	public MemoryKarte(string vorne, int bildID, MemoryFeld spiel) 
        {
            //die Vorderseite, der Dateiname des Bildes wird an den Konstruktor übergeben
		    bildVorne = new Image();
            bildVorne.Source = new BitmapImage(new Uri(vorne,UriKind.Relative));
             //die Rückseite, sie wird fest gesetzt
            bildHinten = new Image();
            bildHinten.Source = new BitmapImage(new Uri("grafiken/verdeckt.bmp", UriKind.Relative));
            //die Eigenschaften zuweisen
            this.Content = bildHinten;

		    //die Bild-ID
		    this.bildID = bildID;
		    //die Karte ist erst einmal umgedreht und noch im Feld
		    umgedreht = false;
		    nochImSpiel = true;
            //mit dem Spielfeld verbinden
            this.spiel = spiel;
		    //die Methode mit dem Ereignis verbinden
            this.Click += new RoutedEventHandler(buttonClick);
	    }

        //die Methode für das Anklicken
        private void buttonClick(object sender, RoutedEventArgs e) 
        {
			//ist die Karte überhaupt noch im Spiel?
            //und sind Züge erlaubt?
            if ((nochImSpiel == false) || (spiel.ZugErlaubt() == false))
				return;
			//wenn die Rückseite zu sehen ist, die Vorderseite anzeigen
			if (umgedreht == false) 
            {
                VorderseiteZeigen();
                //die Methode KarteOeffnen() im Spielfeld aufrufen
                //übergeben wird dabei die Karte - also die this-Referenz
                spiel.KarteOeffnen(this);
			}
        }

	    //die Methode zeigt die Rückseite der Karte an
	    public void RueckseiteZeigen(bool rausnehmen) 
        {
		    //soll die Karten komplett aus dem Spiel genommen werden?
		    if (rausnehmen == true) 
            {
			    //das Bild aufgedeckt zeigen und die Karte aus dem Spiel nehmen
                Image bildRausgenommen = new Image();
                bildRausgenommen.Source = new BitmapImage(new Uri("grafiken/aufgedeckt.bmp", UriKind.Relative));
			    this.Content = bildRausgenommen;
			    nochImSpiel = false;
		    }
		    else 
            {
			    //sonst nur die Rückseite zeigen
			    this.Content = bildHinten;
			    umgedreht = false;
		    }
	    }

	    //die Methode liefert die Bild-ID einer Karte
	    public int GetBildID() 
        {
		    return bildID;
	    }

    	//die Methode liefert die Position einer Karte
	    public int GetBildPos() 
        {
		    return bildPos;
	    }

	    //die Methode setzt die Position einer Karte
    	public void SetBildPos(int bildPos) 
        {
		    this.bildPos = bildPos;
	    }

        //die Methode liefert den Wert des Felds umgedreht
        public bool IsUmgedreht()
        {
            return umgedreht;
        }

        //die Methode liefert den Wert des Feld nochImSpiel
        public bool IsNochImSpiel()
        {
            return nochImSpiel;
        }
        
        //die Methode zeigt die Vorderseite der Karte an
        public void VorderseiteZeigen()
        {
            this.Content = bildVorne;
            umgedreht = true;
        }

        //die Methode zeigt dei Rückzeite der Karte an
        public void RueckseiteZeigen()
        {
            this.Content = bildHinten;
            umgedreht = false;
        }
    }
}
	
package main2;

import processing.core.PApplet;

public class Main {
	
	final static int AQUARIUM = 0;
	final static int PLANETEN = 1;
	final static int TESTS = 2;

	final static float Near = 1;
	final static float Far = 200;

  final static float kamera_distanz_von_zylinder = 1.3f; // Meter
  final static float kamera_hohe_fokuspunkt_von_zylinder = 0.125f; // Mitte  von unten gesehen


	public static void main(String[] args) 
	{
		PApplet.main("main2.CoreApplet");
	}

}

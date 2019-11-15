package main;

import processing.core.PApplet;

public class Main {
	
	final static int AQUARIUM = 0;
	final static int PLANETEN = 1;
	final static int TESTS = 2;

	final static float Near = 1;
	final static float Far = 200;
	
	public static final int bug_mode=3;
	
  final static float kamera_distanz_von_zylinder = 3; // Meter
  final static float kamera_hohe_fokuspunkt_von_zylinder = 4; // Mitte  von unten gesehen


	public static void main(String[] args) 
	{
		PApplet.main("main.CoreApplet");
	}

}

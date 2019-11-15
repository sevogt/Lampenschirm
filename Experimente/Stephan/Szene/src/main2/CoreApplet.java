package main2;

import processing.core.PApplet;
import processing.core.PGraphics;
import processing.core.PImage;
import processing.core.PMatrix;
import processing.core.PShape;
import processing.core.PVector;

import java.awt.Color;
import java.awt.Dimension;
import java.io.IOException;
import java.io.PipedInputStream;
import java.io.PipedOutputStream;
import java.util.ArrayList;
import java.util.concurrent.atomic.AtomicInteger;

import peasy.*;

public class CoreApplet extends PApplet
{
	PeasyCam cam;
	
	private WeltDaten weltDaten;

	float aspekt_ratio;

	int lastTime = 0;
	float delta = 0;

	public PGraphics main_scene;
	public PGraphics buffer2d;
	final boolean fullScreen = true;

	@Override
	public void settings() { 	
	  fullScreen(P3D);

	}

	@Override
	public void setup() 
	{ 

		aspekt_ratio = ((float)width)/((float)height);
		
		weltDaten = new WeltDaten(this);


	  main_scene = createGraphics(width, height,P3D);
    buffer2d = createGraphics(width, height,P2D);

    main_scene.beginDraw();
    main_scene.background(0, 0, 0);
    main_scene.endDraw();
    
    buffer2d.beginDraw();
    buffer2d.background(0, 0, 0);
    buffer2d.endDraw();
		//z - achse geht  >0 nach vorne zum benutzer hin


		//perspective(100, Aspekt , Main.Near, Main.Far);

		cam = new PeasyCam(this, 0, 0, 0, Main.kamera_distanz_von_zylinder);
		//cam.setActive(false );
		cam.setMinimumDistance(1);
		cam.setMaximumDistance(500);

		// muss sein, damit sich die anderen Fenster initialisieren können
		// alternativ kann man AtomicInteger nutzen
		

		lastTime = millis(); 

	}
	

	@Override
	public void draw() 
	{
		background(0, 0, 0);

		float delta = millis() - lastTime;
		lastTime = millis();   
		delta/=1000;

		// ambientLight(50, 50, 50);
		//  lightSpecular(255, 0, 114);
		// spotLight(255, 0, 186, width/2, -3000, 0, width/2.0, 5000, -2000, 1, 60);

				
				PGraphics aGraphics = main_scene;
				PGraphics aBuffer2d = buffer2d;
				weltDaten.simulate();

				{
					PeasyCam aCam = cam;
					weltDaten.render(aCam,aGraphics);	
					aGraphics.perspective(50, aspekt_ratio , Main.Near, Main.Far);
					aCam.getState().apply(aGraphics);

					aBuffer2d.beginDraw();
					aBuffer2d.background(0);
					aBuffer2d.image(aGraphics, 0, 0);
					aBuffer2d.endDraw();

					aCam.beginHUD();
					image(aBuffer2d, 0, 0);
					aCam.endHUD();
				}
			
	
//			text("Click in this window to draw at a relative position in the other window", 10, 10, this.width - 20, 100);
//
//			if(!animation.isEmpty())
//			{
//				PVector vs = animation.get(0);
//
//				float d = point.dist(vs);
//				PVector dir = vs.copy().sub(point);
//
//				if(d<2.8)
//				{
//					dir.setMag((speed/10)*delta);
//				}
//				else
//				{
//					dir.setMag(speed*delta);
//				}
//
//				point.add(dir);
//
//				if(d<0.8)
//				{
//					animation.remove(0);
//				}
//			}
//
//			Color c = new Color(255, 204, 0);  // Define color 'c'
//			fill(c.getRGB());  // Use color variable 'c' as fill color
//			circle(point.x, point.y, 55);
		


	}


	@Override
	public void mousePressed() 
	{
		println("mousePressed in primary window");
		float relPosX = map(mouseX, 0, this.width, 0, 100);
		float relPosY = map(mouseY, 0, this.height, 0, 100);

//	  animation.add(new PVector(mouseX,mouseY));


		//  win1.evokedFromPrimary(relPosX, relPosY);
	}  


}

package main;

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
	
	PipedOutputStream output1;
	PipedOutputStream output2;
	PipedOutputStream output3;
	
	PipedInputStream input1=null;
  PipedInputStream input2=null;
  PipedInputStream input3=null;

	PWindow win1;
	PWindow win2;
	PWindow win3;

	PeasyCam cam;
	
	private WeltDaten weltDaten;
	
	PeasyCam[] cameras;

	AtomicInteger stopper;

	PGraphics dupLayer;
	
	private PGraphics layer_persp_1;
	private PGraphics layer_persp_2;
	private PGraphics layer_persp_3;

	float aspekt_ratio;

	int modus = Main.PLANETEN;

	int lastTime = 0;
	float delta = 0;

	public PGraphics main_scene;
	public PGraphics buffer2d;

	private PWindow[] applets;

	PVector center = new PVector(0,0);

	ArrayList<PVector> animation = new ArrayList();

	PVector point = new PVector(0,0);

	final int my_width = 1200;
	final int my_height = 500;
	
	final int my_width3 = my_width/3;
	
	int the_width_3 = 0;
	int the_height = 0;
	
	
	final boolean fullScreen = true;
	
	
	Dimension dimension = new Dimension(my_width,my_height);

	float speed = 189.9f; // m / s

	@Override
	public void settings() { 	
		if(fullScreen)
		{
			fullScreen(P3D);
		}
		else
		{
			size(dimension.width,dimension.height,P3D);
		}
		
	}

	@Override
	public void setup() 
	{ 
		stopper = new AtomicInteger(6);

		aspekt_ratio = ((float)my_width)/((float)my_height);
		
		weltDaten = new WeltDaten(this);

		applets = new PWindow[3];

	  main_scene = createGraphics(my_width, my_height,P3D);
    buffer2d = createGraphics(my_width, my_height,P2D);
    
    dupLayer = createGraphics(my_width, my_height,P2D);
    
    layer_persp_1 = createGraphics(my_width, my_height,P2D);
    layer_persp_2 = createGraphics(my_width, my_height,P2D);
    layer_persp_3 = createGraphics(my_width, my_height,P2D);

    main_scene.beginDraw();
    main_scene.background(0, 0, 0);
    main_scene.endDraw();
    
    buffer2d.beginDraw();
    buffer2d.background(0, 0, 0);
    buffer2d.endDraw();
    
    if(Main.bug_mode==1)
    {
    	win1 = new PWindow(dimension,this,stopper,null,null);
  		win2 = new PWindow(dimension,this,stopper,null,null);
  		win3 = new PWindow(dimension,this,stopper,null,null);

  		applets[0]=win1;
  		applets[1]=win2;
  		applets[2]=win3;
  		
  		Thread thread1 = new Thread(new Runnable() {	
  			@Override
  			public void run() {
  				PApplet.runSketch(new String[] {this.getClass().getSimpleName()}, win1);

  			}
  		}); 
  		thread1.setDaemon(true);
  		thread1.start();

  		Thread thread2 = new Thread(new Runnable() {	
  			@Override
  			public void run() {
  				PApplet.runSketch(new String[] {this.getClass().getSimpleName()}, win2);

  			}
  		}); 
  		thread2.setDaemon(true);
  		thread2.start();

  		Thread thread3 = new Thread(new Runnable() {	
  			@Override
  			public void run() {
  				PApplet.runSketch(new String[] {this.getClass().getSimpleName()}, win3);

  			}
  		}); 
  		thread3.setDaemon(true);
  		thread3.start();
  		
  		while(stopper.get() != 3)
  		{
  			;
  		}
  		println("ja");
  		
  		cameras = new PeasyCam[3];
  		
  		
  		PeasyCam cam1 = new PeasyCam(this, 0, 0, 0, Main.kamera_distanz_von_zylinder);
      cam1.setActive(false );
      cam1.setMinimumDistance(1);
      cam1.setMaximumDistance(500);  
      
      
      PeasyCam cam2 = new PeasyCam(this, 0, 0, 0, Main.kamera_distanz_von_zylinder);
      cam2.setActive(false );
      cam2.setMinimumDistance(1);
      cam2.setMaximumDistance(500); 
      cam2.rotateY((2f*PI/3f));
      
      PeasyCam cam3 = new PeasyCam(this, 0, 0, 0, Main.kamera_distanz_von_zylinder);
      cam3.setActive(false );
      cam3.setMinimumDistance(1);
      cam3.setMaximumDistance(500); 
      cam3.rotateY((2f*2f*PI/3f));
      
      cameras[0]=cam1;
      cameras[1]=cam2;
      cameras[2]=cam3;
  		
    }
    else if(Main.bug_mode==2)
    {
    	
  		output1 = new PipedOutputStream();
  		output2 = new PipedOutputStream();
  		output3 = new PipedOutputStream();
  		
  		PipedOutputStream o_output1 = new PipedOutputStream();
  		PipedOutputStream o_output2 = new PipedOutputStream();
  		PipedOutputStream o_output3 = new PipedOutputStream();
  		
  		
  		
  	  PipedInputStream o_input1=null;
  	  PipedInputStream o_input2=null;
  	  PipedInputStream o_input3=null;
			try 
			{
				o_input1 = new PipedInputStream(output1);
				o_input2  = new PipedInputStream(output2);
				o_input3  = new PipedInputStream(output3);
				
				input1=new PipedInputStream(o_output1);
	  	  input2=new PipedInputStream(o_output2);
	  	  input3=new PipedInputStream(o_output3);
			} catch (IOException e) {
				e.printStackTrace();
			}
  
    	win1 = new PWindow(dimension,this,stopper,o_input1,o_output1);
  		win2 = new PWindow(dimension,this,stopper,o_input2,o_output2);
  		win3 = new PWindow(dimension,this,stopper,o_input3,o_output3);

  		applets[0]=win1;
  		applets[1]=win2;
  		applets[2]=win3;
  		
  		Thread thread1 = new Thread(new Runnable() {	
  			@Override
  			public void run() {
  				PApplet.runSketch(new String[] {this.getClass().getSimpleName()}, win1);

  			}
  		}); 
  		thread1.setDaemon(true);
  		thread1.start();

  		Thread thread2 = new Thread(new Runnable() {	
  			@Override
  			public void run() {
  				PApplet.runSketch(new String[] {this.getClass().getSimpleName()}, win2);

  			}
  		}); 
  		thread2.setDaemon(true);
  		thread2.start();

  		Thread thread3 = new Thread(new Runnable() {	
  			@Override
  			public void run() {
  				PApplet.runSketch(new String[] {this.getClass().getSimpleName()}, win3);

  			}
  		}); 
  		thread3.setDaemon(true);
  		thread3.start();
  			
  		try {
				int inp1 = input1.read();
				int inp2 = input2.read();
				int inp3 = input3.read();
			} catch (IOException e1) { e1.printStackTrace(); }
  		
  		println("ja");
  		win2.cam.rotateY((2f*PI/3f));
  		win1.cam.rotateY((2f*2f*PI/3f));
  		
  		// start
  		try {
        output1.write(1);
        output2.write(1);
        output3.write(1);
  		} catch (IOException e) {}
    }
    else if(Main.bug_mode==3)
    {

    	if(fullScreen)
  		{
    		the_width_3 = width/3;
    		the_height = height;
  		}
  		else
  		{
  			the_width_3 = my_width3;
    		the_height = my_height;
  		}
    	
    	aspekt_ratio = ((float)the_width_3)/((float)the_height);
    	
  	  main_scene = createGraphics(the_width_3, the_height,P3D);
      buffer2d = createGraphics(the_width_3, the_height,P2D);
      
      layer_persp_1 = createGraphics(the_width_3, the_height,P2D);
      layer_persp_2 = createGraphics(the_width_3, the_height,P2D);
      layer_persp_3 = createGraphics(the_width_3, the_height,P2D);
      
      cameras = new PeasyCam[3];
      
  		PeasyCam cam1 = new PeasyCam(this, 0, 0, 0, Main.kamera_distanz_von_zylinder);
//      cam1.setActive(false );
      cam1.setMinimumDistance(1);
      cam1.setMaximumDistance(500);  
      
      
      PeasyCam cam2 = new PeasyCam(this, 0, 0, 0, Main.kamera_distanz_von_zylinder);
//      cam2.setActive(false );
      cam2.setMinimumDistance(1);
      cam2.setMaximumDistance(500); 
      cam2.rotateY((2f*PI/3f));
      
      PeasyCam cam3 = new PeasyCam(this, 0, 0, 0, Main.kamera_distanz_von_zylinder);
//      cam3.setActive(false );
      cam3.setMinimumDistance(1);
      cam3.setMaximumDistance(500); 
      cam3.rotateY((2f*2f*PI/3f));
      
      cameras[0]=cam1;
      cameras[1]=cam2;
      cameras[2]=cam3;
      
      
    }


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


		if(modus == Main.AQUARIUM)
		{

		}
		else if(modus == Main.PLANETEN)
		{


			if(Main.bug_mode==0)
			{
				PeasyCam aCam =  cam;

				PGraphics aGraphics = main_scene;
				
//				buffer2d = createGraphics(width, height,P2D);
				PGraphics aBuffer2d = buffer2d;

				weltDaten.simulate();
				weltDaten.render(aCam,aGraphics);	
				aGraphics.perspective(100, aspekt_ratio , Main.Near, Main.Far);
				aCam.getState().apply(aGraphics);

				aBuffer2d.beginDraw();
				aBuffer2d.background(0);
				aBuffer2d.image(aGraphics, 0, 0);
				aBuffer2d.endDraw();
				
				dupLayer.copy(aBuffer2d, 0, 0, aBuffer2d.pixelWidth, aBuffer2d.height, 0, 0, dupLayer.width, dupLayer.height);
				
//				aBuffer2d.loadPixels();
//				dupLayer.loadPixels();
//				dupLayer.beginDraw();
//				dupLayer.clear();
//			  for (int i = 0; i < dupLayer.pixels.length; i++) 
//			  {
//			  	dupLayer.pixels[i] = aBuffer2d.pixels[i];
//			  }
//			  //a.updatePixels();
//			  dupLayer.updatePixels();
//			  dupLayer.endDraw();
		
				
//				aBuffer2d.beginDraw();
//				aBuffer2d.loadPixels();
//				aBuffer2d.updatePixels();
//				aBuffer2d.endDraw();
				
//				dupLayer.loadPixels();
				
//				arrayCopy(aBuffer2d.pixels, dupLayer.pixels);
//				dupLayer.updatePixels(); 
//				aBuffer2d.updatePixels(); 
//				
//			PImage img = aBuffer2d.copy();
//				try {
//					img = (PImage) img.clone();
//				} catch (CloneNotSupportedException e) {
//					e.printStackTrace();
//					println("error clone pimage");
//				}

				aCam.beginHUD();
				image(dupLayer, 0, 0);
				aCam.endHUD();
			}	
			else if(Main.bug_mode==1)
	    {
				
				while(stopper.get() != 0)
				{
					;
				}
				stopper.set(3);
				
				for(int i=0;i<applets.length;i++)
				{
					PWindow applet = applets[i];

					PeasyCam aCam =  cameras[i];

					PGraphics aGraphics = main_scene;
					
					buffer2d = createGraphics(my_width, my_width,P2D);
					PGraphics aBuffer2d = buffer2d;

					weltDaten.simulate();
					weltDaten.render(aCam,aGraphics);	
					aGraphics.perspective(100, applet.aspekt_ratio , Main.Near, Main.Far);
					aCam.getState().apply(aGraphics);

//					aBuffer2d.beginDraw();
//					aBuffer2d.background(0);
//					aBuffer2d.image(aGraphics, 0, 0);
//					aBuffer2d.endDraw();
//					
//					applet.buffer2d.beginDraw();
//					applet.buffer2d.copy(aBuffer2d,0,0 ,aBuffer2d.width,aBuffer2d.height,0,0,applet.buffer2d.pixelWidth,applet.buffer2d.height);
//					applet.buffer2d.endDraw();
					
					

					applet.redraw();
					
					aCam.beginHUD();
					image(applet.buffer2d, 0, 0);
					aCam.endHUD();

					
				}
				
	    }
			else if (Main.bug_mode==2)
			{
				
				
				// wait send order to render
				
				try {
					int inp1 = input1.read();
					int inp2 = input2.read();
					int inp3 = input3.read();
				} catch (IOException e1) { e1.printStackTrace(); }
				
				try {
	        output1.write(1);
	        output2.write(1);
	        output3.write(1);
	  		} catch (IOException e) {}
				
			}
			else if (Main.bug_mode==3)
			{
				
				
				PGraphics aGraphics = main_scene;
				PGraphics aBuffer2d = buffer2d;
				weltDaten.simulate();

				{
					PeasyCam aCam = cameras[0];
					weltDaten.render(aCam,aGraphics);	
					aGraphics.perspective(100, aspekt_ratio , Main.Near, Main.Far);
					aCam.getState().apply(aGraphics);

					aBuffer2d.beginDraw();
					aBuffer2d.background(0);
					aBuffer2d.image(aGraphics, 0, 0);
					aBuffer2d.endDraw();

					aCam.beginHUD();
					image(aBuffer2d, 0, 0);
					aCam.endHUD();
				}
				
				{
					PeasyCam aCam = cameras[1];
					weltDaten.render(aCam,aGraphics);	
					aGraphics.perspective(100, aspekt_ratio , Main.Near, Main.Far);

					aCam.getState().apply(aGraphics);

					aBuffer2d.beginDraw();
					aBuffer2d.background(0);
					aBuffer2d.image(aGraphics, 0, 0);
					aBuffer2d.endDraw();

					aCam.beginHUD();
					image(aBuffer2d, the_width_3, 0);
					aCam.endHUD();
				}
				
				{
					PeasyCam aCam = cameras[2];
					weltDaten.render(aCam,aGraphics);	
					aGraphics.perspective(100, aspekt_ratio , Main.Near, Main.Far);

					aCam.getState().apply(aGraphics);

					aBuffer2d.beginDraw();
					aBuffer2d.background(0);
					aBuffer2d.image(aGraphics, 0, 0);
					aBuffer2d.endDraw();

					aCam.beginHUD();
					image(aBuffer2d, the_width_3*2, 0);
					aCam.endHUD();
				}
				
			}
			

		}
		else if(modus == Main.TESTS)
		{
			text("Click in this window to draw at a relative position in the other window", 10, 10, this.width - 20, 100);

			if(!animation.isEmpty())
			{
				PVector vs = animation.get(0);

				float d = point.dist(vs);
				PVector dir = vs.copy().sub(point);

				if(d<2.8)
				{
					dir.setMag((speed/10)*delta);
				}
				else
				{
					dir.setMag(speed*delta);
				}

				point.add(dir);

				if(d<0.8)
				{
					animation.remove(0);
				}
			}

			Color c = new Color(255, 204, 0);  // Define color 'c'
			fill(c.getRGB());  // Use color variable 'c' as fill color
			circle(point.x, point.y, 55);
		}


	}


	@Override
	public void mousePressed() 
	{
		println("mousePressed in primary window");
		float relPosX = map(mouseX, 0, this.width, 0, 100);
		float relPosY = map(mouseY, 0, this.height, 0, 100);


		if(modus == Main.AQUARIUM)
		{

		}
		else if(modus == Main.PLANETEN)
		{

		}
		else if(modus == Main.TESTS)
		{

			animation.add(new PVector(mouseX,mouseY));

		}



		//  win1.evokedFromPrimary(relPosX, relPosY);
	}  


}

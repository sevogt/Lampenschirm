package main;

import java.awt.Dimension;
import java.io.IOException;
import java.io.PipedInputStream;
import java.io.PipedOutputStream;
import java.util.ArrayList;
import java.util.concurrent.atomic.AtomicInteger;

import peasy.PeasyCam;
import processing.core.PApplet;
import processing.core.PGraphics;
import processing.core.PImage;
import processing.core.PVector;

public class PWindow extends PApplet
{
	public float aspekt_ratio;
	public PeasyCam cam;
	
	private WeltDaten weltDaten;
	
	public PGraphics main_scene;
	public PGraphics buffer2d;

	final Dimension dimension;
	
	final CoreApplet coreApplet;
	final AtomicInteger stopper;
	
	final PipedInputStream input;
	final PipedOutputStream output;
	
	final int dist_modifer_camera= 25;

  PWindow(Dimension dimension, CoreApplet coreApplet, AtomicInteger stopper, PipedInputStream input, PipedOutputStream o_output ) 
  {   
		super();  
    this.dimension = dimension;
    this.coreApplet = coreApplet;
    this.stopper=stopper;
    this.input=input;
    this.output = o_output;
  }

	@Override
  public void settings() {
		size(dimension.width,dimension.height,P3D);
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
    if (Main.bug_mode==0)
    {
    	
    }
    else if (Main.bug_mode==1)
    {
    	stopper.decrementAndGet();
    	noLoop(); 
    }
    else if (Main.bug_mode==2)
		{
    	try {
        output.write(1);
  		} catch (IOException e) {}
		}


//    perspective(100, aspekt_ratio , Main.Near, Main.Far);
    cam = new PeasyCam(this, 0, 0, 0, Main.kamera_distanz_von_zylinder+dist_modifer_camera);
//    cam.setActive(false);
    cam.setMinimumDistance(1);
    cam.setMaximumDistance(500);  
    

  	
    
  }

	@Override
  public void draw() 
  {   
		background(0, 0, 0);
		
		 if (Main.bug_mode==0)
	    {
	    	
	    }
	    else if (Main.bug_mode==1)
	    {
	    	cam.beginHUD();
	  	  image(buffer2d, 0, 0);  // buffer2d
	  	  cam.endHUD();
	  	  stopper.decrementAndGet();
	    }
	    else if (Main.bug_mode==2)
			{
	    	try {
					int data = input.read();
				} catch (IOException e) {
					e.printStackTrace();
					println("error");
				}
	    	weltDaten.simulate();
	    	weltDaten.render(cam, main_scene);

	    	main_scene.perspective(100, aspekt_ratio , Main.Near, Main.Far);
				cam.getState().apply(main_scene);

				buffer2d.beginDraw();
				buffer2d.background(0);
				buffer2d.image(main_scene, 0, 0);
				buffer2d.endDraw();
	
	    	cam.beginHUD();
	  	  image(buffer2d, 0, 0);  // buffer2d
	  	  cam.endHUD();
	  	  
	  	  try {
	        output.write(1);
	  		} catch (IOException e) {}
			}
		
//		coreApplet.render(cam,main_scene);
//		
//		main_scene.perspective(100, aspekt_ratio , Main.Near, Main.Far);
//		cam.getState().apply(main_scene);
//
//		buffer2d.beginDraw();
//		buffer2d.background(0);
//		buffer2d.image(main_scene, 0, 0);
//		buffer2d.endDraw();
		
		
  }

	@Override
  public void mousePressed() 
	{
//    println("mousePressed in secondary window");
  }

}

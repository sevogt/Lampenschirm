package main;

import peasy.PeasyCam;
import processing.core.PApplet;
import processing.core.PConstants;
import processing.core.PGraphics;
import processing.core.PImage;
import processing.core.PShape;
import processing.core.PVector;

public class WeltDaten 
{
	private PApplet parent;
	
	public AnimatedObject sun;
	public AnimatedObject planet1;
	
	//Setup - Zylinder
	private final float zylinder_hohe   = 3.7f; // Meter
	private final float zylinder_radius = 2.4f; // Meter
	public AnimatedObject zylinder; 

	private final int iteration_punkte = 200;
	
	
	float rotation_value=0f;

	public WeltDaten(PApplet parent )
	{
		this.parent=parent;
		
		setupZylinder();
		setupAquarium();
		setupPlaneten();
	}
	
	public void simulate()
	{
		rotation_value+=0.1;
		if(rotation_value>parent.TWO_PI)
		{
			rotation_value=0f;
		}
	}
	
	public void render( PeasyCam aCam, PGraphics aGraphics)
	{

		aGraphics.beginDraw();
		{
			aGraphics.background(113,111,111);

			aGraphics.pushMatrix();
			aGraphics.translate(sun.position.x, sun.position.y,sun.position.z);

			aGraphics.shape(sun.shape);
			aGraphics.popMatrix();

			aGraphics.pushMatrix();
			aGraphics.rotateY(rotation_value);
			aGraphics.translate(planet1.position.x,planet1.position.y,planet1.position.z);
			
		

			aGraphics.shape(planet1.shape);
			aGraphics.popMatrix();
			
			aGraphics.pushMatrix();
			aGraphics.translate(zylinder.position.x,zylinder.position.y,zylinder.position.z);

			aGraphics.shape(zylinder.shape);
			aGraphics.popMatrix();
			
			
			
		}
		aGraphics.endDraw();
	}
	
	//Setup - Aquarium
	private void setupAquarium()
	{
		
	}


	private void setupPlaneten()
	{
		PVector pos_sun = new PVector(0,0,0);
		PVector pos_planet1 = new PVector(-1.2f,0,0);


		PImage earth_t = parent.loadImage("earth_burn.jpg");
		PImage sun_t = parent.loadImage("sun_green.jpg");

		parent.beginShape();
		parent.sphereDetail(100);
		PShape shape_sun = parent.createShape(parent.SPHERE,1.2f);
		shape_sun.setTexture(sun_t);
		parent.endShape(parent.CLOSE);
		shape_sun.setStroke(false); 

		parent.beginShape();
		parent.sphereDetail(100);
		PShape shape_planet1 = parent.createShape(parent.SPHERE,0.7f);
		shape_planet1.setTexture(earth_t);
		parent.endShape(parent.CLOSE);
		shape_planet1.setStroke(false); 



		sun= new AnimatedObject(pos_sun,shape_sun);
		planet1= new AnimatedObject(pos_planet1,shape_planet1);
	}



	private void setupZylinder()
	{
		PVector pos_zylinder = new PVector(0,0,0);

		PShape shape = parent.createShape();


		PVector point = new PVector(0f,0f,zylinder_radius);

		float ein_grad = (float)PConstants.TWO_PI/((float)iteration_punkte);

		shape.beginShape(parent.TRIANGLE_STRIP);

		for(int i=0;i< iteration_punkte;i++)
		{
			point.x=0;
			point.y=0;
			point.z=zylinder_radius;
			float rot_winkel = (ein_grad*i);
			
			float temp_x=point.x;

			point.x =  point.x * (float)Math.cos(rot_winkel) +( point.z * (float)Math.sin(rot_winkel));
			point.z = -temp_x * (float)Math.sin(rot_winkel) + (point.z * (float)Math.cos(rot_winkel));

			shape.vertex(point.x,-zylinder_hohe/2f,point.z);
			shape.vertex(point.x, zylinder_hohe/2f,point.z);    
		}

		point.x=0;
		point.y=0;
		point.z=zylinder_radius;
		shape.vertex(point.x,-zylinder_hohe/2,point.z);
		shape.vertex(point.x, zylinder_hohe/2,point.z);

		shape.endShape();

		zylinder = new AnimatedObject(pos_zylinder,shape);
	}


}



// Setup - Aquarium
void setupAquarium()
{
  
}

// Setup - Planeten
final float sonne_radiums = 22;

final int iteration_punkte_kugel = 200;

void setupPlaneten()
{
  PVector pos_sun = new PVector(300,100,0);
  PVector pos_planet1 = new PVector(90,20,20);
  
  PShape shape_sun = createShape(SPHERE,iteration_punkte_kugel);
  PShape shape_planet1 = createShape(SPHERE,iteration_punkte_kugel);

  sun= new AnimatedObject(pos_sun,shape_sun);
  planet1= new AnimatedObject(pos_planet1,shape_planet1);
}

// Setup - Zylinder
final float zylinder_hohe   = 0.7; // Meter
final float zylinder_radius = 0.4; // Meter
AnimatedObject zylinder; 

final int iteration_punkte = 2000;


void setupZylinder()
{
  PVector pos_zylinder = new PVector(0,0,0);
  
  PShape shape = createShape();
 
  
  PVector point = new PVector(0f,0f,zylinder_radius);
  
  float ein_grad = (float)TWO_PI/((float)iteration_punkte);
  
  shape.beginShape();

  for(int i=0;i< iteration_punkte;i++)
  {
    point.x=0;
    point.y=0;
    point.z=zylinder_radius;
    float rot_winkel = (ein_grad*i);
    
    point.x =  point.x * (float)Math.cos(rot_winkel) + point.z * (float)Math.sin(rot_winkel);
    point.z = -point.x * (float)Math.sin(rot_winkel) + point.z * (float)Math.cos(rot_winkel);
    
    shape.vertex(point.x,-zylinder_hohe/2,point.z);
    shape.vertex(point.x, zylinder_hohe/2,point.z);    
  }
  
  point.x=0;
  point.y=0;
  point.z=zylinder_radius;
  shape.vertex(point.x,-zylinder_hohe/2,point.z);
  shape.vertex(point.x, zylinder_hohe/2,point.z);
  
  shape.endShape();
  
  zylinder = new AnimatedObject(pos_zylinder,shape);
}

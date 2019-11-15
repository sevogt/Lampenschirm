
import damkjer.ocd.*;
import peasy.*;

// ## Konstanten ##
PWindow win1;
PWindow win2;
PWindow win3;

final int AQUARIUM = 0;
final int PLANETEN = 1;
final int TESTS = 2;


// ## Updating Data ##


// ## Variables ##
int modus = 1;

int lastTime = 0;
float delta = 0;

// ## Test Data ##
AnimatedObject sun;
AnimatedObject planet1;
PVector center = new PVector(0,0);




ArrayList<PVector> animation = new ArrayList();

PVector point = new PVector(0,0);

float speed = 189.9; // m / s

public void settings() { 
  size(800,600,P3D);
   
}


void setup() { 
  /*
  float distanz_von_zylinder = 3; // Meter
  float hohe_fokuspunkt_von_zylinder = 4; // Mitte
  
  win1 = new PWindow(distanz_von_zylinder,hohe_fokuspunkt_von_zylinder);
  win2 = new PWindow(distanz_von_zylinder,hohe_fokuspunkt_von_zylinder);
  win3 = new PWindow(distanz_von_zylinder,hohe_fokuspunkt_von_zylinder);
  */
  
  // z - achse geht  >0 nach vorne zum benutzer hin
  
  lastTime = millis(); 
  
  setupZylinder();
  
  setupPlaneten();
  
  setupAquarium();

  
}

void draw() {
  background(0, 0, 0);
  float delta = millis() - lastTime;
  lastTime = millis();   
  delta/=1000;
  
  
  if(modus == AQUARIUM)
  {
    
  }
  else if(modus == PLANETEN)
  {
    pushMatrix();
    translate(sun.position.x, sun.position.y,sun.position.z);
   
    shape(sun.shape);
    popMatrix();
    
    pushMatrix();
    translate(planet1.position.x,planet1.position.y,planet1.position.z);
  
    
    shape(planet1.shape);
    popMatrix();
  }
  else if(modus == TESTS)
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
     
    color c = color(255, 204, 0);  // Define color 'c'
    fill(c);  // Use color variable 'c' as fill color
    circle(point.x, point.y, 55);
  }
  

}

void mousePressed() {
  println("mousePressed in primary window");
  float relPosX = map(mouseX, 0, this.width, 0, 100);
  float relPosY = map(mouseY, 0, this.height, 0, 100);
  
    
  if(modus == AQUARIUM)
  {
    
  }
  else if(modus == PLANETEN)
  {
    
  }
  else if(modus == TESTS)
  {
    
    animation.add(new PVector(mouseX,mouseY));
  
  }
  
  

//  win1.evokedFromPrimary(relPosX, relPosY);
}  

class PWindow extends PApplet 
{
  final float distanz_von_zylinder;
  final float hohe_fokuspunkt_von_zylinder;
  ArrayList<PVector> vectors = new ArrayList<PVector>();
  PWindow(float distanz_von_zylinder,float hohe_fokuspunkt_von_zylinder) 
  {   
    super();
    this.distanz_von_zylinder = distanz_von_zylinder;
    this.hohe_fokuspunkt_von_zylinder = hohe_fokuspunkt_von_zylinder;
    
    PApplet.runSketch(new String[] {this.getClass().getSimpleName()}, this);
  }

  void evokedFromPrimary(float relPosX, float relPosY) {
    println("evoked from primary");

    float xPos = map(relPosX, 0, 100, 0, this.width);
    float yPos = map(relPosY, 0, 100, 0, this.height);

    vectors.add(new PVector(xPos, yPos));
  }
  void settings() {
    size(200, 800);
  }

  void setup() {
    background(150);
  }

  void draw() {
    background(150);
    //store the vector size before using to avoid ConcurrentModificationException
    int listLength = vectors.size();
    for(int i = 0; i < listLength; i++) {
      PVector v = vectors.get(i);
      ellipse(v.x, v.y, random(50), random(50));
    }

  }

  void mousePressed() {
    
    println("mousePressed in secondary window");
  }
}

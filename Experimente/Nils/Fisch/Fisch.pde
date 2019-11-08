
float lastX=0, lastY=0;

PShape fish;

void setup() {
  size(1920, 1080, P3D);
  noStroke();
  
  fish = createShape();
  fish.beginShape();
  fish.vertex(0,75,0);
  fish.vertex(0,25,0);
  fish.vertex(75,75,0);
  fish.vertex(100,50,0);
  fish.vertex(75,25,0);
  fish.vertex(0,75,0);
  fish.endShape(CLOSE);
  
}

void draw() {
  println(frameRate);
  lights();
  background(0); 
  noFill();
  shape(fish,mouseX,mouseY);
  
  
  //fill(255,0,0);
  //circle(mouseX, mouseY, 5);
 
}

//void draw() {
//  background(51); 
//  fill(255, 204);
//  rect(mouseX, height/2, mouseY/2+10, mouseY/2+10);
//  fill(255, 204);
//  int inverseX = width-mouseX;
//  int inverseY = height-mouseY;
//  rect(inverseX, height/2, (inverseY/2)+10, (inverseY/2)+10);
//}

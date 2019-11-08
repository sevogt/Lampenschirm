
float lastX=0, lastY=0;

PShape fish, fish2;

float last_x=0, last_y=0;

void setup() {
  size(1920, 1080, P3D);
  noStroke();
  
  //frameRate(5);
  
  fish = createShape();
  fish.beginShape();
  fish.vertex(75,-25,0);
  fish.vertex(75,25,0);
  fish.vertex(25,-25,0);
  fish.vertex(0,0,0);
  fish.vertex(25,25,0);
  fish.endShape(CLOSE);
  
  fish2 = createShape();
  fish2.beginShape();
  fish2.vertex(75,-25,0);
  fish2.vertex(75,25,0);
  fish2.vertex(25,-25,0);
  fish2.vertex(0,0,0);
  fish2.vertex(25,25,0);
  fish2.endShape(CLOSE);
  fish2.rotateY(PI);
}


void draw() {
  
  lights();
  background(0); 
  noFill();
  
  fill(255,0,0);
  textSize(32);
  text("FPS: "+frameRate, 10, 50);
  
  
  //shape(fish,mouseX,mouseY);
  float dx = abs(mouseX - last_x);
  float dy = abs(mouseY - last_y);
  float x, y;
  
  float f_speed=10;
  if(last_x > mouseX) {
    x = last_x - dx/f_speed;
  } else {
    x = last_x + dx/f_speed;
  }
  if(last_y > mouseY) {
    y = last_y - dy/f_speed;
  } else {
    y = last_y + dy/f_speed;
  } 
  last_x = x;
  last_y = y;
  
  
  if(last_y > mouseY) { 
    shape(fish, x, y);
  } else {
    shape(fish2, x, y);
  }
  
  
  fill(255,0,0);
  circle(mouseX, mouseY, 5);
  
  //text("x: "+x+" y: "+y+" mouseX: "+mouseX+" mouseY: "+mouseY, 10, 100);
  
 
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

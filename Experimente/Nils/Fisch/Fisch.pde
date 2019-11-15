<<<<<<< HEAD

float lastX=0, lastY=0;
PShape fish, fish2;
float last_x=0, last_y=0;

PGraphics fish_screen;

//Geklaut bei:
//https://processing.org/examples/texturecylinder.html

int tubeRes = 256;
float[] tubeX = new float[tubeRes];
float[] tubeY = new float[tubeRes];

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
  
  fish_screen = createGraphics(1920,1080,P3D);
  
  float angle = 180.0 / tubeRes;
  for (int i = 0; i < tubeRes; i++) {
    tubeX[i] = cos(radians(i * angle));
    tubeY[i] = sin(radians(i * angle));
  }
  noStroke();
  
}


void draw() {
  
  fish_screen.beginDraw();
  fish_screen.lights();
  fish_screen.background(0,0,255); 
  fish_screen.fill(255,0,0);
  
  //shape(fish,mouseX,mouseY);
  float dx = abs(mouseX - last_x);
  float dy = abs(mouseY - last_y);
  float x, y;
  
  float f_speed=15;
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
  
  
  if(last_x > mouseX) { 
    fish_screen.shape(fish, x, y);
  } else {
    fish_screen.shape(fish2, x, y);
  }
  
  fish_screen.fill(255,0,0);
  fish_screen.circle(mouseX, mouseY, 10);
  fish_screen.endDraw();
  
  background(0);

  pushMatrix();
  translate(width / 2, height / 2);
  rotateY(-PI); //?
  //rotateX(PI/4);
  
  beginShape(QUAD_STRIP);
  texture(fish_screen);
  for (int i = 0; i < tubeRes; i++) {
    float x_t = tubeX[i] * 300;
    float z_t = tubeY[i] * 300;
    float u_t = fish_screen.width / tubeRes * i;
    vertex(x_t, -300, z_t, u_t, 0);
    vertex(x_t, 300, z_t, u_t, fish_screen.height);
  }
  endShape();
  popMatrix();
  
  fill(255,0,0);
  textSize(32);
  text("FPS: "+frameRate, 10, 50);
  
  
 
}
=======

float lastX=0, lastY=0;
PShape fish, fish2;
float last_x=0, last_y=0;

PGraphics fish_screen;

//Geklaut bei:
//https://processing.org/examples/texturecylinder.html

int tubeRes = 128;
float[] tubeX = new float[tubeRes];
float[] tubeY = new float[tubeRes];

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
  
  fish_screen = createGraphics(1920,1080,P3D);
  
  float angle = 180.0 / tubeRes;
  for (int i = 0; i < tubeRes; i++) {
    tubeX[i] = cos(radians(i * angle));
    tubeY[i] = sin(radians(i * angle));
  }
  noStroke();
  
}


void draw() {
  
  fish_screen.beginDraw();
  fish_screen.lights();
  fish_screen.background(0,0,255); 
  fish_screen.fill(255,0,0);
  
  //shape(fish,mouseX,mouseY);
  float dx = abs(mouseX - last_x);
  float dy = abs(mouseY - last_y);
  float x, y;
  
  float f_speed=15;
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
  
  
  if(last_x > mouseX) { 
    fish_screen.shape(fish, x, y);
  } else {
    fish_screen.shape(fish2, x, y);
  }
  
  fish_screen.fill(255,0,0);
  fish_screen.circle(mouseX, mouseY, 10);
  fish_screen.endDraw();
  
  background(0);

  pushMatrix();
  translate(width / 2, height / 2);
  rotateY(-PI); //?
  
  beginShape(QUAD_STRIP);
  texture(fish_screen);
  for (int i = 0; i < tubeRes; i++) {
    float x_t = tubeX[i] * 300;
    float z_t = tubeY[i] * 300;
    float u_t = fish_screen.width / tubeRes * i;
    vertex(x_t, -300, z_t, u_t, 0);
    vertex(x_t, 300, z_t, u_t, fish_screen.height);
  }
  endShape();
  popMatrix();
  
  fill(255,0,0);
  textSize(32);
  text("FPS: "+frameRate, 600, 800);
  
  
 
}
>>>>>>> 57c6af09268ee9686011f5d1c954ef75693ed0c9

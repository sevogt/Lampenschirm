


class AnimatedObject
{
  float speed = 189.9; // m / s
  
  public PVector position;
  public PVector velocity = new PVector(0,0,0);
  public PShape shape;

  
  public AnimatedObject(PVector pos,PShape s)
  {
    position=pos;
    shape=s;
  }

}

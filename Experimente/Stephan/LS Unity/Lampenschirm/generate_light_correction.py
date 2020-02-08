import numpy as np
from PIL import Image


from math import pi, cos, sin, tan

# Radius of Cylinder and distance from projector to Cylinder in cm
radius = 35
distance = 170
path = 'Assets/AusgleichsTextur.png'

width = 2200
height = 1200
angle = 120
max_angle = angle/2 
max_pixel = width/2

def calc_theta(r, d, alpha):
#    print("r: {}, d: {}, alpha: {}".format(r, d, alpha))
    alpha = alpha * pi / 180
    x = sin(alpha) * r
    p = cos(alpha) * r
    phi = tan(x/(d+r-p))
    alpha = alpha * 180 / pi
    phi = phi * 180 / pi
    beta = 180 - alpha - phi
    theta = 180 - beta
    # print("x: {}, p: {}, phi {}, beta: {}, theta: {}".format(x, p, phi, beta,
    #     theta))
    return theta

def pixel_to_angle(x):
    x = abs(x)
    ratio = max_angle/max_pixel
    return x * ratio

def calc_transparency(x):
    x_angle = pixel_to_angle(x) #*pi/180
    theta = calc_theta(radius, distance, x_angle)
    bright_perc = cos(theta*pi/180)
    transparency = 255 * (1-bright_perc)
    return transparency

# Generate the PNG

array = np.zeros([height, width, 4], dtype=np.uint8)
array[:,:] = [0, 0, 0, 255]   # Everything black

# Set transparency depending on x position
middle = width/2

for x in range(int(middle)):
	for y in range(height):
		transparency = calc_transparency(x)
		array[y, x, 3] = transparency
		array[y, int(width-1-x), 3] = transparency

img = Image.fromarray(array)
img.save(path)


import cv2
import numpy as np
from pprint import pprint

def gather_corners(i, c):
	mimg=i.copy()
	def draw_circle(e,x,y,flags,param):
		nonlocal i, c, mimg
		if(e == cv2.EVENT_LBUTTONDOWN):
			if(len(c) < 4):
				cv2.circle(mimg,(x,y),10,(0,0,255),1)
				c.append((x,y))
				cv2.imshow("sframe",mimg)
		elif(e == cv2.EVENT_RBUTTONDOWN):
			c.clear()
			mimg = i.copy()
			cv2.imshow("sframe",mimg)
	return draw_circle

img = cv2.imread("T2.jpg", cv2.IMREAD_COLOR)
corners = []

cv2.namedWindow("sframe")
cv2.setMouseCallback("sframe",gather_corners(img, corners))
cv2.imshow("sframe",img)
cv2.waitKey(0)
cv2.destroyAllWindows()

#### !!!!
#corners = [(78, 0), (794, 125), (1038, 625), (0, 559)] 
####

if(len(corners) != 4):
	print("4 Ecken auswählen!")
	exit(1)

#o_width = img.shape[1]
#o_height = img.shape[0]

n_c = np.zeros((4, 2), dtype="float32")
left = set(sorted(corners, key=lambda x: x[0])[:2])
right = set(sorted(corners, key=lambda x: x[0])[2:])
up = set(sorted(corners, key=lambda x: x[1])[:2])
down = set(sorted(corners, key=lambda x: x[1])[2:])

n_c[0] = (up & left).pop()
n_c[1] = (up & right).pop()
n_c[2] = (down & left).pop()
n_c[3] = (down & right).pop()
t_width = int(max((np.sqrt(np.square(n_c[1][0] - n_c[0][0]) + np.square(n_c[1][1] - n_c[0][1])), np.sqrt(np.square(n_c[3][0] - n_c[2][0]) + np.square(n_c[3][1] - n_c[2][1])))))
t_height = int(max((np.sqrt(np.square(n_c[2][0] - n_c[0][0]) + np.square(n_c[2][1] - n_c[0][1])), np.sqrt(np.square(n_c[3][0] - n_c[1][0]) + np.square(n_c[3][1] - n_c[1][1])))))
mimg = img.copy()
for i in range(4):
	cv2.circle(mimg,(n_c[i][0], n_c[i][1]),10,(0,0,255),1)
	cv2.putText(mimg, str(i), (n_c[i][0], n_c[i][1]), cv2.QT_FONT_NORMAL, 1, (0, 0, 255), 2)

cv2.line(mimg,(10, 10),(10+t_width, 10), (0,255,0), 2)
cv2.line(mimg,(10, 20),(10+t_height, 20), (0,255,0), 2)
cv2.namedWindow("cframe")
cv2.imshow("cframe",mimg)
cv2.waitKey(0)
cv2.destroyAllWindows()

t_c = np.array([[0,0], [t_width - 1, 0], [0, t_height - 1], [t_width - 1, t_height - 1]], dtype="float32")
t_mat = cv2.getPerspectiveTransform(n_c, t_c)

t_img = cv2.warpPerspective(img, t_mat, (t_width, t_height))
cv2.namedWindow("tframe")
cv2.imshow("tframe",t_img)
cv2.waitKey(0)
cv2.destroyAllWindows()

########

o_img = cv2.imread("T1.jpg", cv2.IMREAD_COLOR)
o_img = cv2.resize(o_img, (t_width, t_height))
o_img = cv2.cvtColor(o_img, cv2.COLOR_BGR2GRAY)

c_img = cv2.cvtColor(t_img, cv2.COLOR_BGR2GRAY)

cv2.namedWindow("oframe")
cv2.namedWindow("cframe")
cv2.imshow("oframe",o_img)
cv2.imshow("cframe",c_img)
cv2.waitKey(0)
cv2.destroyAllWindows()


#o_img = cv2.blur(o_img, (5,5))
#c_img = cv2.blur(c_img, (5,5))

d_img = cv2.subtract(o_img, c_img)
d_img = cv2.blur(d_img, (15,15))
#_, d_img = cv2.threshold(d_img, 0, 255, cv2.THRESH_BINARY_INV | cv2.THRESH_OTSU)

d_max = np.max(d_img)
_, d_img = cv2.threshold(d_img, d_max * 0.75, 255, cv2.THRESH_BINARY)

#d_img = cv2.blur(d_img, (25,25))
#_, d_img = cv2.threshold(d_img, 0, 255, cv2.THRESH_BINARY_INV | cv2.THRESH_OTSU)

cv2.namedWindow("dframe")
cv2.imshow("dframe",d_img)
cv2.waitKey(0)
cv2.destroyAllWindows()

contours, hierarchy = cv2.findContours(d_img, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
f_img = t_img.copy()
cv2.drawContours(f_img, contours, 0, (0,255,0), 2)

enc_circles = []
for cont in contours:
	enc_circles.append(cv2.minEnclosingCircle(cont))

xc, xr = max(enc_circles, key = lambda x: x[1])
cv2.circle(f_img, (int(xc[0]), int(xc[1])), int(xr), (0,0,255), 2)

pprint(xc)
cv2.namedWindow("fframe")
cv2.imshow("fframe",f_img)
cv2.waitKey(0)
cv2.destroyAllWindows()

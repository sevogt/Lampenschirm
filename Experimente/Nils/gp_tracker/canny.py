from __future__ import print_function
import cv2 as cv
import argparse
max_lowThreshold = 100
window_name = 'Edge Map'
title_trackbar = 'Min Threshold:'
ratio = 3
kernel_size = 3
ksz=(1,1)

def foo(val):
    global ksz
    ksz=(val, val)

def CannyThreshold(val):
    global low_threshold
    low_threshold = val

cap = cv.VideoCapture(0 + cv.CAP_DSHOW)
    
cap.set(cv.CAP_PROP_FRAME_WIDTH, 1366)
cap.set(cv.CAP_PROP_FRAME_HEIGHT, 768)

low_threshold=0
cv.namedWindow(window_name)
cv.createTrackbar(title_trackbar, window_name , 0, max_lowThreshold, CannyThreshold)
cv.createTrackbar("ksize", window_name , 1, 25, foo)

while True:
    ret, src = cap.read()
    src_gray = cv.cvtColor(src, cv.COLOR_BGR2GRAY)
    
    img_blur = cv.blur(src_gray, ksz)
    detected_edges = cv.Canny(img_blur, low_threshold, low_threshold*ratio, kernel_size)
    mask = detected_edges != 0
    dst = src * (mask[:,:,None].astype(src.dtype))
    print(low_threshold)
    cv.imshow(window_name, dst)
    keyboard = cv.waitKey(30)
    if keyboard == ord('q'):
        break
cv.destroyAllWindows()

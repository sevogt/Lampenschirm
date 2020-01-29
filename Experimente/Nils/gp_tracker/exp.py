import cv2
import numpy as np

def get_t(corners):
    n_c = np.zeros((4, 2), dtype="float32")
    left = set(sorted(corners, key=lambda x: x[0])[:2])
    right = set(sorted(corners, key=lambda x: x[0])[2:])
    up = set(sorted(corners, key=lambda x: x[1])[:2])
    down = set(sorted(corners, key=lambda x: x[1])[2:])

    n_c[0] = (up & left).pop()
    n_c[1] = (up & right).pop()
    n_c[2] = (down & left).pop()
    n_c[3] = (down & right).pop()
    t_width = int(max((np.sqrt(np.square(n_c[1][0] - n_c[0][0]) + np.square(n_c[1][1] - n_c[0][1])), np.sqrt(
        np.square(n_c[3][0] - n_c[2][0]) + np.square(n_c[3][1] - n_c[2][1])))))
    t_height = int(max((np.sqrt(np.square(n_c[2][0] - n_c[0][0]) + np.square(n_c[2][1] - n_c[0][1])), np.sqrt(
        np.square(n_c[3][0] - n_c[1][0]) + np.square(n_c[3][1] - n_c[1][1])))))

    t_c = np.array([[0, 0], [t_width - 1, 0], [0, t_height - 1],
                    [t_width - 1, t_height - 1]], dtype=np.float32)
    t_mat = cv2.getPerspectiveTransform(n_c, t_c)

    return t_mat, (t_height, t_width, 3)

cap = cv2.VideoCapture("D:\\gp_tracker\\Neuer Ordner\\test.avi")

thresh=70
ratio=2.8
ksize_blur=(25,25)
ksize_canny=25
corners_fix = [(1250, 655), (2248, 672), (1259, 1288), (2234, 1279)]
t_mat, t_size = get_t(corners_fix)

cv2.namedWindow("W1", cv2.WINDOW_NORMAL)
cv2.resizeWindow("W1",(t_size[1], t_size[0]))
#cv2.resizeWindow("W1",(1366,768))

cv2.namedWindow("W2", cv2.WINDOW_NORMAL)
cv2.resizeWindow("W2",(t_size[1], t_size[0]))
#cv2.resizeWindow("W2",(1366,768))

cv2.namedWindow("W3", cv2.WINDOW_NORMAL)
cv2.resizeWindow("W3",(t_size[1], t_size[0]))
#cv2.resizeWindow("W3",(1366,768))

f_ctr=0
while(True):
    ret, frame = cap.read()
    f_ctr += 1
    if f_ctr < 100:
        continue

    t_dim = (t_size[1], t_size[0])
    frame = cv2.warpPerspective(frame, t_mat, t_dim)
    frame_gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    #contrast=-127
    #f = 131*(contrast + 127)/(127*(131-contrast))
    #alpha_c = f
    #gamma_c = 127*(1-f)

    #frame_gray = cv2.addWeighted(frame_gray, alpha_c, frame_gray, 0, gamma_c)
    #alpha contrast 0 - 2; beta brightness
    frame_gray = cv2.equalizeHist(frame_gray)
    #frame_gray = cv2.convertScaleAbs(frame_gray, alpha=0.5, beta=1)
    #frame_gray = cv2.adaptiveThreshold(frame_gray,255,cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY,11,2)
    #frame_blur2 = cv2.GaussianBlur(frame_gray,(5,5), 0)
    #_, frame_gray_t = cv2.threshold(frame_blur2,0,255,cv2.THRESH_BINARY+cv2.THRESH_OTSU)

    #frame_gray = frame
    frame_blur = cv2.blur(frame_gray, ksize_blur)
    _, frame_gray_t = cv2.threshold(frame_blur,30,255,cv2.THRESH_BINARY)

    detected_edges = cv2.Canny(frame_blur, thresh, thresh*ratio, ksize_canny)
    mask = detected_edges != 0
    frame_canny = frame * (mask[:,:,None].astype(frame.dtype))

    # frame_resz = cv2.resize(detected_edges, (1366,768))
    # frame_resz2 = cv2.resize(frame, (1366, 768))
    # frame_resz3 = cv2.resize(frame_blur, (1366,768))
    frame_resz = detected_edges
    frame_resz2 = frame_gray_t
    frame_resz3 = frame_blur
    cv2.imshow("W1", frame)
    cv2.imshow("W2", frame_resz2)
    cv2.imshow("W3", frame_resz3)
    if cv2.waitKey(1) == ord('q'):
        break

cv2.destroyAllWindows()

'''

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
'''
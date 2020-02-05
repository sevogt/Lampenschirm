import cv2
import numpy as np
from pprint import pprint

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

####
x=np.array([[[228,310]],[[227,311]],[[195,311]],[[194,312]],[[192,312]],[[191,313]],[[190,313]],[[189,312]],[[188,312]],
[[187,311]],[[118,311]],[[117,312]],[[40,312]],[[39,313]],[[37,313]],[[36,314]],[[35,314]],[[34,315]],[[20,315]],[[19,316]],
[[16,316]],[[15,317]],[[13,317]],[[12,318]],[[10,318]],[[9,319]],[[0,319]],[[0,519]],[[575,519]],[[575,487]],[[574,486]],[[574,485]],
[[572,483]],[[572,481]],[[571,480]],[[571,478]],[[570,477]],[[570,476]],[[568,474]],[[567,474]],[[565,472]],[[564,472]],[[563,471]]
,[[561,471]],[[560,470]],[[558,470]],[[557,469]],[[554,469]],[[547,462]],[[546,462]],[[543,459]],[[542,459]],[[541,458]],[[540,458]]
,[[539,457]],[[538,457]],[[536,455]],[[535,455]],[[534,454]],[[533,454]],[[532,453]],[[507,453]],[[506,452]],[[506,450]],[[502,446]]
,[[501,446]],[[497,442]],[[496,442]],[[494,440]],[[493,440]],[[492,439]],[[491,439]],[[490,438]],[[489,438]],[[487,436]],[[487,435]]
,[[486,434]],[[486,433]],[[485,432]],[[485,431]],[[484,430]],[[484,429]],[[483,428]],[[483,426]],[[482,425]],[[482,417]],[[481,416]],
[[481,415]],[[480,414]],[[480,412]],[[478,410]],[[478,409]],[[463,394]],[[462,394]],[[460,392]],[[459,392]],[[458,391]],[[457,391]],
[[444,378]],[[444,377]],[[438,371]],[[438,370]],[[434,366]],[[434,365]],[[432,363]],[[432,362]],[[428,358]],[[428,357]],[[423,352]],
[[422,352]],[[418,348]],[[417,348]],[[416,347]],[[415,347]],[[414,346]],[[412,346]],[[411,345]],[[409,345]],[[408,344]],[[407,344]],
[[406,343]],[[403,343]],[[402,342]],[[376,342]],[[375,341]],[[373,341]],[[372,340]],[[371,340]],[[370,339]],[[369,339]],[[367,337]],
[[366,337]],[[365,336]],[[364,336]],[[363,335]],[[361,335]],[[360,334]],[[359,334]],[[358,333]],[[357,333]],[[355,331]],[[354,331]],
[[353,330]],[[352,330]],[[351,329]],[[350,329]],[[349,328]],[[348,328]],[[347,327]],[[345,327]],[[344,326]],[[343,326]],[[342,325]],
[[341,325]],[[340,324]],[[338,324]],[[337,323]],[[328,323]],[[327,322]],[[321,322]],[[320,321]],[[317,321]],[[316,320]],[[315,320]],
[[314,319]],[[313,319]],[[312,318]],[[311,318]],[[309,316]],[[308,316]],[[307,315]],[[306,315]],[[304,313]],[[303,313]],[[302,312]],
[[300,312]],[[299,311]],[[294,311]],[[293,310]]],dtype=np.int32)
pprint(x[:, :, 0])
y = np.sqrt( ( np.square( x[:,:,0] - 300 ) + np.square( x[:,:,1] - 300) ) )
z = np.argmin(y)
pprint(y)
pprint(z)
x_zz, y_zz = x[z,0,]
pprint((x_zz,y_zz))
exit(0)

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
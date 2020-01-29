import cv2
import numpy as np
from pprint import pprint
import socket
from multiprocessing import Process, SimpleQueue


def send_results(results_q):
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    serv_t = (SRV_ADDR, SRV_PORT)

    while True:
        res = results_q.get()
        if res is None:
            break
        msg = "{} {}".format(*res).encode("ascii")
        try:
            sock.sendto(msg, serv_t)
        except InterruptedError:
            pass
    sock.close()


def gather_corners(corners, f_size, w_size):
    f_x = float(f_size[0])/float(w_size[0])
    f_y = float(f_size[1])/float(w_size[1])
    def store_corner(e, x, y, flags, param):
        nonlocal corners
        if(e == cv2.EVENT_LBUTTONDOWN and len(corners) < 4):
            corners.append((int(float(x)*f_x), int(float(y)*f_y)))
        elif(e == cv2.EVENT_RBUTTONDOWN):
            corners.clear()
    return store_corner


def prepare_cam(cap_s):
    cap, cap_size = cap_s
    frame = np.zeros(cap_size, dtype=np.float32)
    window_size=(1366, 768)
    frame_window = np.zeros((window_size[1], window_size[0], 3), dtype=np.float32)
    corners = []
    cv2.namedWindow("Einstellungen")
    cv2.namedWindow("Einstellungen", cv2.WINDOW_NORMAL)
    cv2.resizeWindow("Einstellungen",(1366,768))
    cv2.setMouseCallback("Einstellungen", gather_corners(corners, (cap_size[1], cap_size[0]), window_size))
    while True:
        retval, frame = cap.read(image=frame)
        if not retval:
            print("Err 2")
            exit(1)
        for corner in corners:
            cv2.circle(frame, corner, 20, (0, 0, 255), 5)
        frame_window =  cv2.resize(frame, window_size, dst=frame_window)
        cv2.imshow("Einstellungen", frame_window)
        if cv2.waitKey(1) == ord('w'):
            break

    cv2.destroyAllWindows()
    if(len(corners) != 4):
        print("Err 3")
        exit(1)
    else:
        return corners


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


def check_cam(cap_s, t_mat, t_size):
    cap, cap_size = cap_s
    frame = np.zeros(cap_size, dtype=np.float32)
    t_dim = (t_size[1], t_size[0])
    cv2.namedWindow("Kontrolle")
    dst_frame = np.zeros(t_size, dtype=np.float32)
    while True:
        ret, frame = cap.read(image=frame)
        if not ret:
            print("Err 4")
            exit(1)
        dst_frame = cv2.warpPerspective(frame, t_mat, t_dim, dst_frame)
        cv2.imshow("Kontrolle", dst_frame)
        wait_key = cv2.waitKey(1)
        if wait_key == ord('w'):
            cv2.destroyAllWindows()
            return True
        elif wait_key == ord("z"):
            cv2.destroyAllWindows()
            return False

def detect3(cap_s, ignore, t_mat, t_size, results_q):
    cap, cap_size = cap_s
    t_dim = (t_size[1], t_size[0])
    cv2.namedWindow("Detektion")
    cam_frame = np.zeros(cap_size, dtype=np.float32)
    dst_frame = np.zeros(t_size, dtype=np.float32)
    gry_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.float32)
    eql_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.float32)
    edg_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.float32)
    while True:
        ret, cam_frame = cap.read(image=cam_frame)
        if not ret:
            print("Err 5")
            return
        dst_frame = cv2.warpPerspective(cam_frame, t_mat, t_dim, dst_frame)
        gry_frame = cv2.cvtColor(dst_frame, cv2.COLOR_BGR2GRAY, gry_frame)
        eql_frame = cv2.equalizeHist(gry_frame, eql_frame)
        
        
        #edg_frame = cv2.medianBlur(eql_frame, 25, edg_frame)
        edg_frame = cv2.blur(eql_frame, (11, 11), edg_frame)
        #edg_frame = cv2.GaussianBlur(eql_frame, (11, 11), 0, edg_frame)
        edg_frame = cv2.Laplacian(eql_frame, cv2.CV_32F,edg_frame)

        cv2.imshow("Detektion", edg_frame)
        if cv2.waitKey(1) == ord('q'):
            return

def detect2(cap_s, background_sub, t_mat, t_size, results_q):
    cap, cap_size = cap_s
    t_dim = (t_size[1], t_size[0])
    cv2.namedWindow("Detektion")
    cam_frame = np.zeros(cap_size, dtype=np.float32)
    dst_frame = np.zeros(t_size, dtype=np.float32)
    gry_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.float32)
    eql_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.float32)
    fg_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.float32)
    while True:
        ret, cam_frame = cap.read(image=cam_frame)
        if not ret:
            print("Err 5")
            return
        dst_frame = cv2.warpPerspective(cam_frame, t_mat, t_dim, dst_frame)
        gry_frame = cv2.cvtColor(dst_frame, cv2.COLOR_BGR2GRAY, gry_frame)
        eql_frame = cv2.equalizeHist(gry_frame, eql_frame)
        
        fg_frame = background_sub.apply(eql_frame,fgmask=fg_frame)
        cv2.imshow("Detektion", fg_frame)
        if cv2.waitKey(1) == ord('q'):
            return


def detect(cap_s, classfier, t_mat, t_size, results_q):
    cap, cap_size = cap_s
    t_dim = (t_size[1], t_size[0])
    cv2.namedWindow("Detektion")
    cam_frame = np.zeros(cap_size, dtype=np.float32)
    dst_frame = np.zeros(t_size, dtype=np.float32)
    gry_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.float32)
    eql_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.float32)
    while True:
        ret, cam_frame = cap.read(image=cam_frame)
        if not ret:
            print("Err 5")
            return
        dst_frame = cv2.warpPerspective(cam_frame, t_mat, t_dim, dst_frame)
        gry_frame = cv2.cvtColor(dst_frame, cv2.COLOR_BGR2GRAY, gry_frame)
        eql_frame = cv2.equalizeHist(gry_frame, eql_frame)
        detected_objs = classfier.detectMultiScale(eql_frame)
        center_prc = (-1., -1.)
        for (x, y, w, h) in detected_objs:
            center = (x + w//2, y + h//2)
            center_prc = (min((float(center[0])/float(t_dim[0]), 1.)),
                          min((float(center[1])/float(t_dim[1]), 1.)))
            frame = cv2.ellipse(dst_frame, center,
                                (w//2, h//2), 0, 0, 360, (0, 0, 255), 4)
        results_q.put(center_prc)
        cv2.imshow("Detektion", dst_frame)
        if cv2.waitKey(1) == ord('q'):
            return


def main():
    classfier = cv2.CascadeClassifier()
    if not classfier.load(CC_URI):
        print("Err 0")
        exit(1)

    if ALGO == 'MOG2':
        background_sub = cv2.createBackgroundSubtractorMOG2()
    else:
        background_sub = cv2.createBackgroundSubtractorKNN()
        background_sub.setNSamples(100)
        background_sub.setShadowThreshold(0.3)

    cap = cv2.VideoCapture(CAM_ID + cv2.CAP_DSHOW)
    #cap = cv2.VideoCapture("D:\\gp_tracker\\Neuer Ordner\\test.avi")

    if not cap.isOpened:
        print("Err 1")
        exit(1)

    cap.set(cv2.CAP_PROP_FRAME_WIDTH, 3840)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 2160)

    v_size = (int(cap.get(cv2.CAP_PROP_FRAME_HEIGHT)),
              int(cap.get(cv2.CAP_PROP_FRAME_WIDTH)), 3)

    cap_s = (cap, v_size)

    while True:
        corners = prepare_cam(cap_s)
        t_mat, t_size = get_t(corners)
        if check_cam(cap_s, t_mat, t_size):
            break

    pprint(corners)

    results_q = SimpleQueue()
    server_process = Process(target=send_results, args=(results_q,))
    server_process.start()

    #detect(cap_s, classfier, t_mat, t_size, results_q)
    #detect2(cap_s,background_sub,t_mat,t_size,results_q)
    detect3(cap_s, None, t_mat, t_size, results_q)

    results_q.put(None)
    cap.release()
    server_process.join()


CAM_ID = 0
CC_URI = "D:\opencv3.4.8\sources\data\haarcascades\haarcascade_frontalface_default.xml"
SHOW_IMG = True
SRV_ADDR = "localhost"
SRV_PORT = 54321
ALGO="KNN"
#ALGO="MOG2"

if __name__ == "__main__":
    main()
# cont_dist = []
            # x_l, y_l = main_cont_center
            # for i in range(len(largest_cont)):
            #     cont_dist.append((i, math.sqrt((centers_cont[i][0] - x_l)**2+(centers_cont[i][1] - y_l)**2)))
            # cont_dist.sort(key=lambda x: x[1])
            # greatest_dist = -1
            # split_idx = -1
            # for i in range(len(cont_dist) - 1):
            #     cur_dist = abs(cont_dist[i+1][1] - cont_dist[i][1])
            #     if cur_dist >= greatest_dist:
            #         greatest_dist = cur_dist
            #         split_idx = i + 1
            # selected_points = []
            # selected_conts = []
            # for i, _ in cont_dist[:split_idx]:
            #     selected_points.append(centers_cont[i])
            #     selected_conts.append(largest_cont[i])
            
            # pprint((len(largest_cont), split_idx, len(selected_conts)))

            # cv2.drawContours(warp_frame, cont, -1, (0,255,255), 3)
            # cv2.drawContours(warp_frame, selected_conts, -1, (255,255,0), 3)
            # cv2.drawContours(warp_frame, [main_cont], -1, (0,0,255), 3)

            #if len(selected_points) > 2:
                #cv2.drawContours(warp_frame, cont, -1, (0,255,255), 3)
                #cv2.drawContours(warp_frame, selected_conts, -1, (255,255,0), 3)
                #cv2.drawContours(warp_frame, [main_cont], -1, (0,0,255), 3)
                #for i in range(len(selected_conts)):
                #    cv2.circle(warp_frame, selected_points[i], 3, (255, 0, 255), 3)
                #cv2.circle(warp_frame, main_cont_center, 3, (0,255,0),3)
                #(x,y) = (x0,y0) + t*(vx,vy)
                #v_x, v_y, x_0, y_0 = cv2.fitLine(np.array(selected_points), cv2.DIST_L2, 0, 0.01, 0.01)
                
                #m=250
                #cv2.line(warp_frame, (int(x_l-m*v_x[0]), int(y_l-m*v_y[0])), (int(x_l+m*v_x[0]), int(y_0+m*v_y[0])), (255,255,0), 3)
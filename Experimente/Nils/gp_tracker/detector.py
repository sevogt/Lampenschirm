
from pprint import pprint

import cv2
import numpy as np
import socket
import math
from multiprocessing import Process, SimpleQueue

from pprint import pprint

CAM_ID = 0
CAM_W = 3840
CAM_H = 2160
WIN_W = 1366
WIN_H = 768
CAM_DRIVER = cv2.CAP_DSHOW
SHOW_IMG = True
ALGO = "MOG2"
CAM_ID = 0
SRV_ADDR = "localhost"
SRV_PORT = 54321

DEBUG = True
if DEBUG:
    CAM_ID = "D:\\gp_tracker\\Neuer Ordner\\test.avi"
    CAM_DRIVER = ""


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
    frame = np.zeros(cap_size, dtype=np.uint8)
    window_size = (WIN_W, WIN_H)
    frame_window = np.zeros(
        (window_size[1], window_size[0], 3), dtype=np.uint8)
    corners = []
    cv2.namedWindow("Einstellungen")
    cv2.namedWindow("Einstellungen", cv2.WINDOW_NORMAL)
    cv2.resizeWindow("Einstellungen", window_size)
    cv2.setMouseCallback("Einstellungen", gather_corners(
        corners, (cap_size[1], cap_size[0]), window_size))
    while True:
        retval, _ = cap.read(image=frame)
        if not retval:
            print("Err 2")
            exit(1)
        for corner in corners:
            cv2.circle(frame, corner, 20, (0, 0, 255), 5)
        cv2.resize(frame, window_size, dst=frame_window)
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
    frame = np.zeros(cap_size, dtype=np.uint8)
    t_dim = (t_size[1], t_size[0])
    cv2.namedWindow("Kontrolle")
    dst_frame = np.zeros(t_size, dtype=np.uint8)
    while True:
        ret, _ = cap.read(image=frame)
        if not ret:
            print("Err 4")
            exit(1)
        cv2.warpPerspective(frame, t_mat, t_dim, dst=dst_frame)
        cv2.imshow("Kontrolle", dst_frame)
        wait_key = cv2.waitKey(1)
        if wait_key == ord('w'):
            cv2.destroyAllWindows()
            return True
        elif wait_key == ord("z"):
            cv2.destroyAllWindows()
            return False


def get_trackbar_update_blur_k_sz(detection_params):
    def update_blur_k_sz(val):
        nonlocal detection_params
        detection_params["k_sz"] = (val + 1, val + 1)
    return update_blur_k_sz


def get_trackbar_update_thresh(detection_params):
    def update_thresh(val):
        nonlocal detection_params
        detection_params["thresh"] = val
    return update_thresh


def get_trackbar_update_eql_hist(detection_params):
    def update_eql_hist(val):
        nonlocal detection_params
        detection_params["eql_h"] = True if val == 1 else False
    return update_eql_hist


def get_trackbar_update_dil(detection_params):
    def update_dil(val):
        nonlocal detection_params
        detection_params["dil"] = True if val == 1 else False
    return update_dil


def get_trackbar_update_dil_sz(detection_params):
    def update_dil_sz(val):
        nonlocal detection_params
        detection_params["dil_k_sz"] = val + 1
    return update_dil_sz


def get_trackbar_update_dil_iter(detection_params):
    def update_dil_iter(val):
        nonlocal detection_params
        detection_params["dil_iter"] = val + 1
    return update_dil_iter


def get_params_thresh_blur(cap_s, t_mat, t_size, back_sub):
    cap, cap_size = cap_s
    frame = np.zeros(cap_size, dtype=np.uint8)
    warp_frame = np.zeros(t_size, dtype=np.uint8)
    grey_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    hist_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    blur_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    thresh_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    fg_mask = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    fg_dilate = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    t_dim = (t_size[1], t_size[0])
    
    detection_params = {'eql_h': False, 'k_sz': (25, 25), 'thresh': 121, "sub_lr": -1, "dil": True,
                         "dil_type": cv2.MORPH_RECT, "dil_k_sz": 4, "dil_iter": 3, "min_c_sz": 24*100}

    window_name = "Einstellung Parameter"
    window_name_ctrl = "Kontrolle"
    window_name_bg_sep = "Vordergrund"
    cv2.namedWindow(window_name)
    cv2.namedWindow(window_name_ctrl)
    cv2.namedWindow(window_name_bg_sep)
    tr_blur_name = "Blur Kernel Size + 1"
    tr_thresh_name = "Threshold"
    tr_eql_hist_name = "Equalize Hist"
    cv2.createTrackbar(tr_blur_name, window_name, detection_params["k_sz"][0] - 1, 49,
                       get_trackbar_update_blur_k_sz(detection_params))
    cv2.createTrackbar(tr_thresh_name, window_name, detection_params["thresh"], 254,
                       get_trackbar_update_thresh(detection_params))
    cv2.createTrackbar(tr_eql_hist_name, window_name, 1 if detection_params["eql_h"] else 0, 1,
                       get_trackbar_update_eql_hist(detection_params))

    tr_dil_name = "Dilate"
    tr_dil_sz_name = "Kernel sz 2n+1"
    tr_dil_iter_name = "Iter"
    cv2.createTrackbar(tr_dil_name, window_name_bg_sep, 1 if detection_params["dil"] else 0, 1,
                       get_trackbar_update_dil(detection_params))
    cv2.createTrackbar(tr_dil_sz_name, window_name_bg_sep, detection_params["dil_k_sz"], 30,
                       get_trackbar_update_dil_sz(detection_params))
    cv2.createTrackbar(tr_dil_iter_name, window_name_bg_sep, detection_params["dil_iter"], 10,
                       get_trackbar_update_dil_iter(detection_params))

    while True:
        retval, _ = cap.read(image=frame)
        if not retval:
            print("Err 5")
            exit(1)

        cv2.warpPerspective(frame, t_mat, t_dim, dst=warp_frame)
        cv2.cvtColor(warp_frame, cv2.COLOR_BGR2GRAY, dst=grey_frame)

        if detection_params["eql_h"]:
            cv2.equalizeHist(grey_frame, dst=hist_frame)
        else:
            np.copyto(hist_frame, grey_frame)

        cv2.blur(hist_frame, detection_params["k_sz"], dst=blur_frame)
        _, _ = cv2.threshold(
            blur_frame, detection_params["thresh"], 255, cv2.THRESH_BINARY, dst=thresh_frame)

        back_sub.apply(thresh_frame, fgmask=fg_mask,
                       learningRate=detection_params["sub_lr"])

        dil_struct_elem = cv2.getStructuringElement(detection_params["dil_type"], (2*detection_params["dil_k_sz"] + 1, 2 *
                                                                                   detection_params["dil_k_sz"]+1), (detection_params["dil_k_sz"], detection_params["dil_k_sz"]))
        if detection_params["dil"]:
            cv2.dilate(fg_mask, dil_struct_elem, dst=fg_dilate,
                       iterations=detection_params["dil_iter"])
        else:
            np.copyto(fg_dilate, fg_mask)

        cv2.imshow(window_name_bg_sep, fg_dilate)
        cv2.imshow(window_name, thresh_frame)
        cv2.imshow(window_name_ctrl, hist_frame)
        if cv2.waitKey(1) == ord('w'):
            break

    cv2.destroyAllWindows()
    return detection_params


def check_params(cap_s, t_mat, t_size, detection_params, back_sub):
    cap, cap_size = cap_s
    frame = np.zeros(cap_size, dtype=np.uint8)
    warp_frame = np.zeros(t_size, dtype=np.uint8)
    grey_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    hist_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    blur_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    thresh_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    fg_mask = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    fg_dilate = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    t_dim = (t_size[1], t_size[0])

    window_name = "Kontrolle Parameter"
    window_name_ctrl = "Kontrolle Parameter Warp"
    cv2.namedWindow(window_name)

    while True:
        ret, _ = cap.read(image=frame)
        if not ret:
            print("Err 6")
            exit(1)

        cv2.warpPerspective(frame, t_mat, t_dim, dst=warp_frame)
        cv2.cvtColor(warp_frame, cv2.COLOR_BGR2GRAY, dst=grey_frame)
        if detection_params["eql_h"]:
            cv2.equalizeHist(grey_frame, dst=hist_frame)
        else:
            np.copyto(hist_frame, grey_frame)
        cv2.blur(hist_frame, detection_params["k_sz"], dst=blur_frame)
        _, _ = cv2.threshold(
            blur_frame, detection_params["thresh"], 255, cv2.THRESH_BINARY, dst=thresh_frame)
        back_sub.apply(thresh_frame, fgmask=fg_mask,
                       learningRate=detection_params["sub_lr"])
        dil_struct_elem = cv2.getStructuringElement(detection_params["dil_type"], (2*detection_params["dil_k_sz"] + 1, 2 *
                                                                                detection_params["dil_k_sz"]+1), (detection_params["dil_k_sz"],
                                                                                detection_params["dil_k_sz"]))
        if detection_params["dil"]:
            cv2.dilate(fg_mask, dil_struct_elem, dst=fg_dilate,
                       iterations=detection_params["dil_iter"])
        else:
            np.copyto(fg_dilate, fg_mask)

        cv2.imshow(window_name, fg_dilate)
        cv2.imshow(window_name_ctrl, warp_frame)
        wait_key = cv2.waitKey(1)
        if wait_key == ord('w'):
            cv2.destroyAllWindows()
            return True
        elif wait_key == ord("z"):
            cv2.destroyAllWindows()
            return False


def get_trackbar_update_min_c_sz(detection_params):
    def update_min_c_sz(val):
        nonlocal detection_params
        detection_params["min_c_sz"] = (val + 1)*100
    return update_min_c_sz


def get_params_min_cont_sz(cap_s, t_mat, t_size, detection_params, back_sub):
    cap, cap_size = cap_s
    frame = np.zeros(cap_size, dtype=np.uint8)
    warp_frame = np.zeros(t_size, dtype=np.uint8)
    grey_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    hist_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    blur_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    thresh_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    fg_mask = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    fg_dilate = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    t_dim = (t_size[1], t_size[0])

    window_name = "Einstellung Min Contour Sz"
    cv2.namedWindow(window_name)
    tr_min_c_sz_name = "Min Cont Sz x100"
    cv2.createTrackbar(tr_min_c_sz_name, window_name, int(detection_params["min_c_sz"]/100), 100,
                       get_trackbar_update_min_c_sz(detection_params))

    dil_struct_elem = cv2.getStructuringElement(detection_params["dil_type"], (2*detection_params["dil_k_sz"] + 1, 2 *
                                                detection_params["dil_k_sz"]+1), (detection_params["dil_k_sz"], detection_params["dil_k_sz"]))

    while True:
        ret, _ = cap.read(image=frame)
        if not ret:
            print("Err 8")
            exit(1)

        cv2.warpPerspective(frame, t_mat, t_dim, dst=warp_frame)
        cv2.cvtColor(warp_frame, cv2.COLOR_BGR2GRAY, dst=grey_frame)
        if detection_params["eql_h"]:
            cv2.equalizeHist(grey_frame, dst=hist_frame)
        else:
            np.copyto(hist_frame, grey_frame)
        cv2.blur(hist_frame, detection_params["k_sz"], dst=blur_frame)
        _, _ = cv2.threshold(
            blur_frame, detection_params["thresh"], 255, cv2.THRESH_BINARY, dst=thresh_frame)
        back_sub.apply(thresh_frame, fgmask=fg_mask,
                       learningRate=detection_params["sub_lr"])
        if detection_params["dil"]:
            cv2.dilate(fg_mask, dil_struct_elem, dst=fg_dilate,
                       iterations=detection_params["dil_iter"])
        else:
            np.copyto(fg_dilate, fg_mask)

        _, cont, cont_h = cv2.findContours(fg_dilate, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

        largest_cont = []
        for c in cont:
            cont_area = cv2.contourArea(c)
            if cont_area > detection_params["min_c_sz"]:
                largest_cont.append(c)

        cv2.drawContours(warp_frame, largest_cont, -1, (0,255,0), 3)
        
        cv2.imshow(window_name, warp_frame)
        wait_key = cv2.waitKey(1)
        if wait_key == ord('w'):
            cv2.destroyAllWindows()
            return


def detect(cap_s, t_mat, t_size, detection_params, back_sub, result_q):
    cap, cap_size = cap_s
    frame = np.zeros(cap_size, dtype=np.uint8)
    warp_frame = np.zeros(t_size, dtype=np.uint8)
    grey_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    hist_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    blur_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    thresh_frame = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    fg_mask = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    fg_dilate = np.zeros((t_size[0], t_size[1], 1), dtype=np.uint8)
    t_dim = (t_size[1], t_size[0])

    window_name = "Detektion"
    cv2.namedWindow(window_name)

    dil_struct_elem = cv2.getStructuringElement(detection_params["dil_type"], (2*detection_params["dil_k_sz"] + 1, 2 *
                                                detection_params["dil_k_sz"]+1), (detection_params["dil_k_sz"], detection_params["dil_k_sz"]))

    while True:
        ret, _ = cap.read(image=frame)
        if not ret:
            print("Err 9")
            exit(1)

        center_prc = (-1., -1.)

        cv2.warpPerspective(frame, t_mat, t_dim, dst=warp_frame)
        cv2.cvtColor(warp_frame, cv2.COLOR_BGR2GRAY, dst=grey_frame)
        if detection_params["eql_h"]:
            cv2.equalizeHist(grey_frame, dst=hist_frame)
        else:
            np.copyto(hist_frame, grey_frame)
        cv2.blur(hist_frame, detection_params["k_sz"], dst=blur_frame)
        _, _ = cv2.threshold(
            blur_frame, detection_params["thresh"], 255, cv2.THRESH_BINARY, dst=thresh_frame)
        back_sub.apply(thresh_frame, fgmask=fg_mask,
                       learningRate=detection_params["sub_lr"])
        if detection_params["dil"]:
            cv2.dilate(fg_mask, dil_struct_elem, dst=fg_dilate,
                       iterations=detection_params["dil_iter"])
        else:
            np.copyto(fg_dilate, fg_mask)

        _, cont, cont_h = cv2.findContours(fg_dilate, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

        main_cont = None
        main_cont_area = -1
        main_cont_center = None
        largest_cont = []
        centers_cont = []
        for c in cont:
            cont_area = cv2.contourArea(c)
            moments = cv2.moments(c)
            if cont_area > detection_params["min_c_sz"]:
                largest_cont.append(c)
                moments = cv2.moments(c)
                c_cont = (int(moments['m10']/moments['m00']), int(moments['m01']/moments['m00']))
                centers_cont.append(c_cont)
                if main_cont is None or main_cont_area < cont_area:
                    main_cont = c
                    main_cont_area = cont_area
                    main_cont_center = c_cont

        if main_cont is not None:
            bounding_boxes = []
            max_ratio_box_idx = -1
            max_ratio = -1
            for i in range(len(largest_cont)):
                bounding_box = cv2.minAreaRect(largest_cont[i])
                bounding_boxes.append(bounding_box)
                ratio = max(bounding_box[1]) / min(bounding_box[1])
                if ratio > max_ratio:
                    max_ratio = ratio
                    max_ratio_box = i
                box = cv2.boxPoints(bounding_box)
                box = np.int0(box)
                cv2.drawContours(warp_frame,[box],0,(0,0,255),2)

            box = cv2.boxPoints(bounding_boxes[max_ratio_box_idx])
            box = np.int0(box)
            cv2.drawContours(warp_frame,[box],0,(0,255,0),2)
            
            center_prc = (min((float(centers_cont[max_ratio_box_idx][0])/float(t_dim[0]), 1.)),
                          min((float(centers_cont[max_ratio_box_idx][1])/float(t_dim[1]), 1.)))

            cv2.circle(warp_frame, (int(center_prc[0]*t_dim[0]), int(center_prc[1]*t_dim[1])), 3, (0,255,255),3)
            
        result_q.put(center_prc)
        cv2.imshow(window_name, warp_frame)
        wait_key = cv2.waitKey(1)
        if wait_key == ord('q'):
            break
        
    cv2.destroyAllWindows()

def main():
    cap = cv2.VideoCapture(CAM_ID + CAM_DRIVER)
    if not cap.isOpened:
        print("Err 1")
        exit(1)

    cap.set(cv2.CAP_PROP_FRAME_WIDTH, CAM_W)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, CAM_H)

    v_size = (int(cap.get(cv2.CAP_PROP_FRAME_HEIGHT)),
              int(cap.get(cv2.CAP_PROP_FRAME_WIDTH)), 3)
    cap_s = (cap, v_size)

    while True:
        corners = prepare_cam(cap_s)
        t_mat, t_size = get_t(corners)
        if check_cam(cap_s, t_mat, t_size):
            break

    if DEBUG:
        t_mat = np.array([[1.02724247e+00, -5.05201217e-03, -1.28678118e+03],
                          [3.17488226e-03,  1.02654526e+00, -7.02038432e+02],
                          [1.81218700e-06,  1.88211898e-05,  1.00000000e+00]], dtype=np.float32)
        t_size = (611, 981, 3)

    if ALGO == 'MOG2':
        back_sub = cv2.createBackgroundSubtractorMOG2(detectShadows=False)
    else:
        back_sub = cv2.createBackgroundSubtractorKNN(detectShadows=False)

    while True:
        detection_params = get_params_thresh_blur(
            cap_s, t_mat, t_size, back_sub)
        if check_params(cap_s, t_mat, t_size, detection_params, back_sub):
            break

    get_params_min_cont_sz(cap_s, t_mat, t_size, detection_params, back_sub)
    if DEBUG:
        detection_params = {'eql_h': False, 'k_sz': (
            25, 25), 'thresh': 121, "sub_lr": -1, "dil": True, "dil_type": cv2.MORPH_RECT, "dil_k_sz": 4, "dil_iter": 3, "min_c_sz": 24*100}

    results_q = SimpleQueue()
    server_process = Process(target=send_results, args=(results_q,))
    server_process.start()

    detect(cap_s, t_mat, t_size, detection_params, back_sub, results_q)

    results_q.put(None)
    cap.release()
    server_process.join()

    pprint("t_mat = ")
    pprint(t_mat)
    pprint("t_size = ")
    pprint(t_size)
    pprint("detection_params = ")
    pprint(detection_params)


if __name__ == "__main__":
    main()

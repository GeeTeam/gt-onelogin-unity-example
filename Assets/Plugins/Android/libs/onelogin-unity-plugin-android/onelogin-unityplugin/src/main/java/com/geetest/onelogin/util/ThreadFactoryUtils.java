package com.geetest.onelogin.util;

import com.geetest.common.support.NonNull;

import java.util.UUID;
import java.util.concurrent.ThreadFactory;

/**
 * 线程工厂类，生成以OneLogin开头+3位随机UUID的线程名称的线程
 *
 * @author geetest 雷进锋
 * @date 2019/12/24
 */
public class ThreadFactoryUtils {

    private volatile static ThreadFactory threadFactory;

    public static ThreadFactory getInstance() {
        if (threadFactory == null) {
            synchronized (ThreadFactoryUtils.class) {
                if (threadFactory == null) {
                    threadFactory = new ThreadFactory() {
                        /**
                         * 设置线程名字
                         */
                        @Override
                        public Thread newThread(@NonNull Runnable r) {
                            Thread thread = new Thread(r);
                            thread.setName("OneLogin" + UUID.randomUUID().toString().substring(0, 3));
                            return thread;
                        }
                    };
                }
            }
        }
        return threadFactory;
    }
}

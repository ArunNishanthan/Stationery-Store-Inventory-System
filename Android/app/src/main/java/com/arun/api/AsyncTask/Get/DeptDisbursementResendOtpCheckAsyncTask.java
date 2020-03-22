package com.arun.api.AsyncTask.Get;

import android.os.AsyncTask;

import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

public class DeptDisbursementResendOtpCheckAsyncTask extends AsyncTask<String, Void, Void> {

    public DeptDisbursementResendOtpCheckAsyncTask() {
    }

    @Override
    protected Void doInBackground(String... Strings) {
        String id = Strings[0];
        StringBuilder response = new StringBuilder();
        try {
            URL url = new URL("http://10.0.2.2:1256/api/Item/ResentOTP?Id=" + id);
            HttpURLConnection conn = (HttpURLConnection) url.openConnection();
            conn.setRequestMethod("GET");
            try {
                InputStream inputStream = new BufferedInputStream(conn.getInputStream());
                BufferedReader r = new BufferedReader(new InputStreamReader(inputStream));
                for (String line; (line = r.readLine()) != null; ) {
                    response.append(line);
                }

            } catch (Exception e) {
                e.printStackTrace();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return null;
    }


}
package com.arun.api.AsyncTask.Get;

import android.os.AsyncTask;

import com.arun.api.Model.RequisationForm;
import com.google.gson.Gson;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;

public class DeptDisbursementOtpCheckAsyncTask extends AsyncTask<String, Void, String> {
    public IDeptDiscallback icallback;

    public DeptDisbursementOtpCheckAsyncTask(IDeptDiscallback icallback) {
        this.icallback = icallback;
    }

    @Override
    protected String doInBackground(String... Strings) {
        String id = Strings[0];
        String otp = Strings[1];

        JSONObject jsonObj = null;
        StringBuilder response = new StringBuilder();
        try {
            URL url = new URL("http://10.0.2.2:1256/api/Item/ValidateOTP?Id=" + id + "&OTP=" + otp);
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
        return  response.toString();
    }


    @Override
    protected void onPostExecute(String string) {
        if (string == null)
            icallback.getStatus(-1);
        try {
            int status = Integer.parseInt(string) ;
            icallback.getStatus(status);
        } catch (Exception e) {
        }

    }

    public interface IDeptDiscallback {
        void getStatus(int status);
    }
}
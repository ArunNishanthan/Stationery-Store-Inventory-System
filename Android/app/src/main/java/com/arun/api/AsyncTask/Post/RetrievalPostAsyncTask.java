package com.arun.api.AsyncTask.Post;

import android.os.AsyncTask;

import com.arun.api.ViewModel.RetrivalViewModel;

import com.google.gson.Gson;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.ProtocolException;
import java.net.URL;

public class RetrievalPostAsyncTask extends AsyncTask<RetrivalViewModel, Void, Void> {
    @Override
    protected Void doInBackground(RetrivalViewModel... retrivalViewModel) {
        try {
            URL url = new URL("http://10.0.2.2:1256/api/Item/GenerateDisbursementLists");
            HttpURLConnection conn = (HttpURLConnection) url.openConnection();

            if (retrivalViewModel[0] != null) {
                conn.setDoOutput(true);
                conn.setRequestMethod("POST");
                conn.setRequestProperty("Content-Type", "application/json;charset=UTF-8");
                conn.setRequestProperty("Accept", "application/json");
                conn.connect();
                Gson gson = new Gson();
                String jsonInputString = gson.toJson(retrivalViewModel[0]);

                DataOutputStream outstream = new DataOutputStream(conn.getOutputStream());
                outstream.writeBytes(jsonInputString);
                outstream.flush();
                outstream.close();
                StringBuilder response = new StringBuilder();
                InputStream inputStream = new BufferedInputStream(conn.getInputStream());
                BufferedReader r = new BufferedReader(new InputStreamReader(inputStream));
                for (String line; (line = r.readLine()) != null; ) {
                    response.append(line).append('\n');
                }
            }
        } catch (ProtocolException e) {
            e.printStackTrace();
        } catch (MalformedURLException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
        return null;
    }
}


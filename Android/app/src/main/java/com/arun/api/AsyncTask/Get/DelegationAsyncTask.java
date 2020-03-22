package com.arun.api.AsyncTask.Get;

import android.os.AsyncTask;

import com.arun.api.ViewModel.AssignDeptRepViewModel;
import com.arun.api.ViewModel.DelegateAuthorityViewModel;
import com.google.gson.Gson;

import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

public class DelegationAsyncTask extends AsyncTask<String, Void, JSONObject> {
    public Icallback icallback;

    public DelegationAsyncTask(Icallback icallback) {
        this.icallback = icallback;
    }

    @Override
    protected JSONObject doInBackground(String... strings) {
        JSONObject jsonObj = null;
        StringBuilder response = new StringBuilder();
        try {
            String id = strings[0];
            URL url = new URL("http://10.0.2.2:1256/api/Employee/DelegateAuthorityForm?empId=" + id);
            HttpURLConnection conn = (HttpURLConnection) url.openConnection();
            conn.setRequestMethod("GET");
            try {
                InputStream inputStream = new BufferedInputStream(conn.getInputStream());
                BufferedReader r = new BufferedReader(new InputStreamReader(inputStream));
                for (String line; (line = r.readLine()) != null; ) {
                    response.append(line).append('\n');
                }
                jsonObj = new JSONObject(response.toString());
                jsonObj.put("context", "set");
            } catch (Exception e) {
                e.printStackTrace();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return jsonObj;
    }

    @Override
    protected void onPostExecute(JSONObject jsonObj) {
        if (jsonObj == null)
            icallback.getDelegateModel(null);
        try {
            String context = (String) jsonObj.get("context");
            if (context.compareTo("set") == 0) {
                Gson gson = new Gson();
                DelegateAuthorityViewModel delegateAuthorityViewModel=gson.fromJson(jsonObj.toString(),DelegateAuthorityViewModel.class);
                icallback.getDelegateModel(delegateAuthorityViewModel);
            }
        } catch (Exception e) {
System.out.println(e.getMessage());
        }

    }

    public interface Icallback {
        void getDelegateModel(DelegateAuthorityViewModel delegateAuthorityViewModel);
    }
}

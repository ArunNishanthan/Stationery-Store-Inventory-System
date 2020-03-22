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

public class ApprovalAsyncTask extends AsyncTask<String, Void, JSONObject> {
    public IApprovalcallback icallback;

    public ApprovalAsyncTask(IApprovalcallback icallback) {
        this.icallback = icallback;
    }

    @Override
    protected JSONObject doInBackground(String... Strings) {
        String id =Strings[0];
        JSONObject jsonObj = null;
        StringBuilder response = new StringBuilder();
        try {
            URL url = new URL("http://10.0.2.2:1256/api/Item/ApprovalRequestList?empId="+id);
            HttpURLConnection conn = (HttpURLConnection) url.openConnection();
            conn.setRequestMethod("GET");
// receive response
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
            icallback.getRequisationForms(null);
        try {
            String context = (String) jsonObj.get("context");
            if (context.compareTo("set") == 0) {
                JSONArray jArray = (JSONArray)jsonObj.get("RequisationForms");
                ArrayList<RequisationForm> requisationForms = new  ArrayList<RequisationForm>();
                if (jArray != null) {
                    for (int i=0;i<jArray.length();i++){
                        Gson gson = new Gson();
                        RequisationForm requisationForm = gson.fromJson(jArray.getJSONObject(i).toString(),RequisationForm.class);
                        requisationForms.add(requisationForm);
                    }
                }
                icallback.getRequisationForms(requisationForms);
            }
        } catch (Exception e) {
        }

    }

    public interface IApprovalcallback {
        void getRequisationForms(List<RequisationForm> requisationForms);
    }
}
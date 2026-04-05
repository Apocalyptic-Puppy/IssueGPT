# Kubernetes 部署指南

在 Mac 本地 Kubernetes 上部署 IssueGPT，使用 port-forward 訪問。

---

## 前置需求

```bash
# 檢查 Kubernetes 是否已安裝
kubectl version

# 啟用 Kubernetes (在 Docker Desktop 中)
# Preferences → Kubernetes → Enable Kubernetes
```

---

## Step 1: 配置密鑰

編輯 `k8s/secret.yaml` 並添加你的 token：

```bash
nano k8s/secret.yaml
```

修改這些行：

```yaml
GITHUB_TOKEN: "ghp_YOUR_GITHUB_TOKEN_HERE"
OPENAI_API_KEY: "sk-YOUR_OPENAI_KEY_HERE"
```

保存。

---

## Step 2: 構建 Docker 鏡像

```bash
# 構建 API 鏡像
docker build -f Dockerfile.api -t issuegpt-api:latest .

# 構建 Frontend 鏡像
docker build -f Dockerfile.frontend -t issuegpt-frontend:latest .

# 驗證鏡像
docker images | grep issuegpt
```

---

## Step 3: 創建 StorageClass (本地開發)

```bash
# 創建本地 storage class
cat << 'EOF' | kubectl apply -f -
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: local-storage
provisioner: kubernetes.io/no-provisioner
volumeBindingMode: WaitForFirstConsumer
EOF
```

---

## Step 4: 部署到 Kubernetes

### 4.1 創建命名空間

```bash
kubectl apply -f k8s/namespace.yaml
```

### 4.2 部署配置與密鑰

```bash
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
```

### 4.3 部署 MSSQL

```bash
# 創建 PV/PVC
kubectl apply -f k8s/mssql-storage.yaml

# 部署 StatefulSet
kubectl apply -f k8s/mssql-statefulset.yaml

# 等待 MSSQL 啟動 (30-60 秒)
kubectl wait --for=condition=ready pod -l app=mssql -n issuegpt --timeout=300s
```

### 4.4 運行數據庫遷移

獲取 MSSQL pod 名稱並執行遷移：

```bash
# 查找 MSSQL pod
kubectl get pods -n issuegpt -l app=mssql

# 執行數據庫命令 (進入 pod)
kubectl exec -it mssql-0 -n issuegpt -- /bin/bash

# 在 pod 中，使用 sqlcmd (或等待 API 自動執行遷移)
```

### 4.5 部署 API

```bash
kubectl apply -f k8s/api-deployment.yaml

# 等待 pod 啟動
kubectl wait --for=condition=ready pod -l app=issuegpt-api -n issuegpt --timeout=300s

# 檢查日誌
kubectl logs -f deployment/issuegpt-api -n issuegpt
```

### 4.6 部署 Frontend

```bash
kubectl apply -f k8s/frontend-deployment.yaml

# 等待 pod 啟動
kubectl wait --for=condition=ready pod -l app=issuegpt-frontend -n issuegpt --timeout=300s

# 檢查日誌
kubectl logs -f deployment/issuegpt-frontend -n issuegpt
```

---

## Step 5: Port-Forward 訪問

在新的終端窗口中執行：

### 前端 (localhost:3000)

```bash
kubectl port-forward -n issuegpt svc/issuegpt-frontend 3000:80
```

### API (localhost:5000)

```bash
kubectl port-forward -n issuegpt svc/issuegpt-api 5000:80
```

### MSSQL (localhost:1433) - 可選

```bash
kubectl port-forward -n issuegpt svc/mssql 1433:1433
```

---

## Step 6: 訪問應用

- **前端**: http://localhost:3000
- **API Swagger**: http://localhost:5000/swagger
- **MSSQL**: localhost:1433 (user: sa, password: IssueGPT@2026)

---

## 常用命令

### 檢查狀態

```bash
# 查看所有資源
kubectl get all -n issuegpt

# 查看 pod 日誌
kubectl logs -f pod/<pod-name> -n issuegpt

# 檢查事件
kubectl describe pod <pod-name> -n issuegpt

# 進入 pod
kubectl exec -it <pod-name> -n issuegpt -- /bin/bash
```

### 重啟

```bash
# 重啟 API Deployment
kubectl rollout restart deployment/issuegpt-api -n issuegpt

# 重啟 Frontend Deployment
kubectl rollout restart deployment/issuegpt-frontend -n issuegpt
```

### 清理

```bash
# 刪除整個應用
kubectl delete namespace issuegpt

# 清理 MSSQL 數據 (謹慎!)
rm -rf /tmp/mssql-data
```

---

## 一鍵部署腳本

保存為 `deploy.sh`：

```bash
#!/bin/bash

echo "Creating StorageClass..."
cat << 'EOF' | kubectl apply -f -
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: local-storage
provisioner: kubernetes.io/no-provisioner
volumeBindingMode: WaitForFirstConsumer
EOF

echo "Deploying IssueGPT..."
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/mssql-storage.yaml
kubectl apply -f k8s/mssql-statefulset.yaml

echo "Waiting for MSSQL..."
kubectl wait --for=condition=ready pod -l app=mssql -n issuegpt --timeout=300s

kubectl apply -f k8s/api-deployment.yaml
echo "Waiting for API..."
kubectl wait --for=condition=ready pod -l app=issuegpt-api -n issuegpt --timeout=300s

kubectl apply -f k8s/frontend-deployment.yaml
echo "Waiting for Frontend..."
kubectl wait --for=condition=ready pod -l app=issuegpt-frontend -n issuegpt --timeout=300s

echo ""
echo "✅ IssueGPT deployed successfully!"
echo ""
echo "Port-forward commands:"
echo "Frontend:  kubectl port-forward -n issuegpt svc/issuegpt-frontend 3000:80"
echo "API:       kubectl port-forward -n issuegpt svc/issuegpt-api 5000:80"
echo "MSSQL:     kubectl port-forward -n issuegpt svc/mssql 1433:1433"
echo ""
echo "Access:"
echo "Frontend: http://localhost:3000"
echo "API:      http://localhost:5000/swagger"
```

執行：

```bash
chmod +x deploy.sh
./deploy.sh
```

---

## 故障排除

### Pod 無法啟動

```bash
# 查看 pod 狀態
kubectl describe pod <pod-name> -n issuegpt

# 查看事件
kubectl get events -n issuegpt --sort-by='.lastTimestamp'
```

### MSSQL 連接超時

```bash
# 檢查 MSSQL pod 日誌
kubectl logs mssql-0 -n issuegpt

# 確保 StorageClass 存在
kubectl get storageclass
```

### API 無法連接 MSSQL

確保 DNS 名稱正確：

- Service DNS: `mssql.issuegpt.svc.cluster.local`
- 檢查環境變量

### Frontend 無法調用 API

確認 CORS 配置或 proxy 設置。在 nginx.conf 中檢查 `/api/` 代理。

---

## 下一步

### 生產部署

```bash
# 推送鏡像到容器倉庫 (DockerHub/ACR/ECR)
docker tag issuegpt-api:latest yourregistry/issuegpt-api:v1.0
docker push yourregistry/issuegpt-api:v1.0

# 在生產 K8s 集群上部署
# 更新 image 欄位指向遠程倉庫
```

### Ingress 設置 (可選)

```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: issuegpt-ingress
  namespace: issuegpt
spec:
  rules:
    - host: issuegpt.local
      http:
        paths:
          - path: /
            pathType: Prefix
            backend:
              service:
                name: issuegpt-frontend
                port:
                  number: 80
          - path: /api
            pathType: Prefix
            backend:
              service:
                name: issuegpt-api
                port:
                  number: 80
```

---

## 相關文件

- [`Dockerfile.api`](../Dockerfile.api) — API 鏡像構建
- [`Dockerfile.frontend`](../Dockerfile.frontend) — Frontend 鏡像構建
- [`k8s/`](.) — 所有 Kubernetes manifests

---

**IssueGPT 現在運行在 Kubernetes 上！** 🚀

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

## Step 7: 使用 k9s 監控服務

### 安裝 k9s

k9s 是一個 Kubernetes 終端 UI，方便實時監控和管理 Pod、Service、Deployment 等。

**macOS**

```bash
brew install k9s
```

**Linux**

```bash
curl https://github.com/derailed/k9s/releases/download/v0.32.4/k9s_Linux_amd64.tar.gz | tar xz
sudo mv k9s /usr/local/bin/
```

### 啟動 k9s

```bash
# 全局視圖
k9s

# 只看 issuegpt 命名空間
k9s -n issuegpt
```

### k9s 快速導航

| 快捷鍵     | 功能                        |
| ---------- | --------------------------- |
| `:po`      | 查看 Pods                   |
| `:svc`     | 查看 Services               |
| `:dep`     | 查看 Deployments            |
| `:sts`     | 查看 StatefulSets           |
| `:pvc`     | 查看 PersistentVolumeClaims |
| `:cm`      | 查看 ConfigMaps             |
| `:secrets` | 查看 Secrets                |
| `:events`  | 查看事件日誌                |
| `l`        | 查看 Pod 日誌               |
| `d`        | 描述資源詳情                |
| `e`        | 編輯資源                    |
| `shift+f`  | Port-forward                |
| `?`        | 幫助菜單                    |
| `q`        | 退出                        |

### 監控 IssueGPT

```bash
# 1. 進入 k9s
k9s -n issuegpt

# 2. 查看所有 Pods
#    按 `:po` 然後 Enter
#    應看到：
#    - mssql-0 (Ready: 1/1)
#    - issuegpt-api-xxxxx (Ready: 1/1)
#    - issuegpt-api-xxxxx (Ready: 1/1)
#    - issuegpt-frontend-xxxxx (Ready: 1/1)
#    - issuegpt-frontend-xxxxx (Ready: 1/1)

# 3. 查看 Pod 日誌
#    選中 Pod → 按 `l`
#    API Pod 應看到：
#    - "Application started"
#    - HTTP 請求

# 4. 查看 Deployments
#    按 `:dep` 然後 Enter
#    - Ready: 2/2 (表示 2 個副本就緒)
#    - Up to date: 2
#    - Available: 2

# 5. 查看 Services 和端口
#    按 `:svc` 然後 Enter
#    - issuegpt-frontend (Port: 80)
#    - issuegpt-api (Port: 80)
#    - mssql (Headless, Port: 1433)
```

### 實時監控資源使用

k9s 會顯示每個 Pod 的 CPU 和 Memory 使用情況。預期值：

- **API Pod**: CPU 100-250m (限制 500m), Memory 256-512Mi (限制 1Gi)
- **Frontend Pod**: CPU 50-100m (限制 250m), Memory 64-128Mi (限制 256Mi)
- **MSSQL Pod**: CPU 200-400m (限制 1000m), Memory 512-1024Mi (限制 2Gi)

### 故障診斷

```bash
# Pod 無法啟動？
# 1. 進入 k9s → 選中 Pod → 按 `d` 查看詳情
# 2. 查看 Events 欄位，找到具體錯誤
# 3. 按 `l` 查看 Pod 日誌，了解啟動過程

# 例如常見錯誤：
# - ImagePullBackOff: Docker 鏡像未構建
# - CrashLoopBackOff: 應用啟動失敗，查看日誌
# - Pending: 資源不足，查看 Events
```

### 進階操作

```bash
# 編輯運行中的資源
# 選中資源 → 按 `e` 編輯
# 修改後保存，Pod 會自動重啟應用配置

# 從 k9s 內部設置 port-forward
# 選中 Service → 按 `shift+f` → 輸入本地端口

# 查看集群級別事件
# 按 `:events` 查看最新發生的事件
```

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

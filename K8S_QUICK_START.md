# 🚀 IssueGPT 本地 Kubernetes 快速部署

## 簡明步驟 (3 分鐘)

### 1️⃣ 配置密鑰

```bash
# 編輯 secret 文件
nano k8s/secret.yaml

# 修改這兩行：
GITHUB_TOKEN: "ghp_YOUR_TOKEN"
OPENAI_API_KEY: "sk-YOUR_KEY"
```

### 2️⃣ 一鍵部署

```bash
chmod +x deploy-k8s.sh
./deploy-k8s.sh
```

等待 3-5 分鐘，直到所有 pod 就緒。

### 3️⃣ Port-Forward (3 個終端)

**終端 1 - 前端**

```bash
kubectl port-forward -n issuegpt svc/issuegpt-frontend 3000:80
```

**終端 2 - API**

```bash
kubectl port-forward -n issuegpt svc/issuegpt-api 5000:80
```

**終端 3 - MSSQL (可選)**

```bash
kubectl port-forward -n issuegpt svc/mssql 1433:1433
```

### 4️⃣ 訪問

打開瀏覽器：

- **前端**: http://localhost:3000
- **API Docs**: http://localhost:5000/swagger

---

## 📊 使用 k9s 監控服務

**k9s** 是一個 Kubernetes 終端 UI 工具，可以實時監控和管理集群。

### 安裝 k9s

**macOS**

```bash
brew install k9s
```

**Linux**

```bash
curl https://github.com/derailed/k9s/releases/download/v0.32.4/k9s_Linux_amd64.tar.gz | tar xz
sudo mv k9s /usr/local/bin/
```

### 快速開始

```bash
# 啟動 k9s
k9s

# 如果指定命名空間
k9s -n issuegpt
```

### 常用快捷鍵

| 快捷鍵 | 功能                  |
| ------ | --------------------- |
| `:po`  | 查看所有 Pods         |
| `:svc` | 查看所有 Services     |
| `:dep` | 查看所有 Deployments  |
| `:sts` | 查看所有 StatefulSets |
| `:pvc` | 查看持久化卷聲明      |
| `d`    | 查看 Pod 日誌         |
| `e`    | 編輯資源              |
| `l`    | 查看資源標籤          |
| `desc` | 查看資源描述          |
| `?`    | 查看幫助菜單          |

### 監控 IssueGPT 服務

```bash
# 啟動 k9s 並進入 issuegpt 命名空間
k9s -n issuegpt

# 在 k9s 中的操作：
# 1. 查看 Pods
#    按 `:po` 再按 Enter，查看所有 Pods 狀態
#    - issuegpt-mssql-0 (MSSQL 服務器)
#    - issuegpt-api-xxxxx (API Pod #1)
#    - issuegpt-api-xxxxx (API Pod #2)
#    - issuegpt-frontend-xxxxx (Frontend Pod #1)
#    - issuegpt-frontend-xxxxx (Frontend Pod #2)

# 2. 查看 Pod 日誌
#    選中 Pod，按 `l` 查看日誌
#    API Pod 日誌應顯示：
#    - "Application started"
#    - "Database connection successful"
#    - HTTP 請求

# 3. 監控 Deployments
#    按 `:dep` 查看 Deployment 狀態
#    - Ready: 2/2 (表示 2 個副本都已就緒)
#    - Up to date: 2
#    - Available: 2

# 4. 查看 Services
#    按 `:svc` 查看 Service 暴露的端口
#    - issuegpt-api: ClusterIP (Port 80 → 5000)
#    - issuegpt-frontend: ClusterIP (Port 80)
#    - mssql: Headless Service (Port 1433)
```

### 實時性能監控

在 k9s 中查看 Pod 資源使用情況：

```bash
# 啟動 k9s，進入 Pod 列表
k9s -n issuegpt

# 查看 CPU 和 Memory 使用情況：
# - API Pod 應使用 100-250m CPU (限制 500m)
# - Frontend Pod 應使用 50-100m CPU (限制 250m)
# - MSSQL Pod 應使用 200-400m CPU (限制 1000m)
```

### 故障診斷

```bash
# 1. Pod 不就緒？
#    - 選中 Pod，按 `l` 查看日誌，找到錯誤信息
#    - 按 `desc` 查看事件日誌，看是否有 ImagePullBackOff 等

# 2. 查看 MSSQL 連接問題
#    - 選中 mssql-0 Pod，按 `l` 查看啟動日誌
#    - 應看到 "SQL Server started" 消息

# 3. 查看 API 配置問題
#    - 選中 API Pod，按 `l` 查看日誌
#    - 檢查是否有 "Connection string" 或 "OPENAI" 相關錯誤
```

### 進階使用

```bash
# 1. 編輯運行中的 ConfigMap
#    按 `:cm` → 選中 issuegpt → 按 `e` 編輯
#    （謹慎使用，重新啟動 Pod 後才生效）

# 2. 查看 Events 日誌
#    按 `:events` 查看集群事件
#    - Pod scheduling 失敗
#    - Image pull 問題
#    - Resource 不足

# 3. Port-forward from k9s
#    選中 Service，按 `shift+f` 自動設置 port-forward
```

---

## 完整步驟

### 前置設置

```bash
# 檢查 Kubernetes 是否運行
kubectl cluster-info

# 檢查 Docker 鏡像
docker images | grep issuegpt
```

### 部署流程

```bash
# 1. 構建鏡像
docker build -f Dockerfile.api -t issuegpt-api:latest .
docker build -f Dockerfile.frontend -t issuegpt-frontend:latest .

# 2. 創建 StorageClass
cat << 'EOF' | kubectl apply -f -
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: local-storage
provisioner: kubernetes.io/no-provisioner
volumeBindingMode: WaitForFirstConsumer
EOF

# 3. 逐步部署
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/mssql-storage.yaml
kubectl apply -f k8s/mssql-statefulset.yaml

# 等待 MSSQL 就緒
kubectl wait --for=condition=ready pod -l app=mssql -n issuegpt --timeout=300s

kubectl apply -f k8s/api-deployment.yaml
kubectl wait --for=condition=ready pod -l app=issuegpt-api -n issuegpt --timeout=300s

kubectl apply -f k8s/frontend-deployment.yaml
kubectl wait --for=condition=ready pod -l app=issuegpt-frontend -n issuegpt --timeout=300s

# 4. Port-forward (3 個終端)
# 終端 1
kubectl port-forward -n issuegpt svc/issuegpt-frontend 3000:80

# 終端 2
kubectl port-forward -n issuegpt svc/issuegpt-api 5000:80

# 終端 3 (可選)
kubectl port-forward -n issuegpt svc/mssql 1433:1433
```

---

## 監控與調試

### 查看資源狀態

```bash
# 所有資源
kubectl get all -n issuegpt

# 只看 pods
kubectl get pods -n issuegpt -w

# 詳細信息
kubectl describe pod <pod-name> -n issuegpt
```

### 查看日誌

```bash
# API 日誌
kubectl logs -f deployment/issuegpt-api -n issuegpt

# Frontend 日誌
kubectl logs -f deployment/issuegpt-frontend -n issuegpt

# MSSQL 日誌
kubectl logs -f statefulset/mssql -n issuegpt

# 特定 pod 日誌
kubectl logs -f pod/<pod-name> -n issuegpt
```

### 進入 Pod 調試

```bash
# 進入 API pod
kubectl exec -it deployment/issuegpt-api -n issuegpt -- /bin/bash

# 進入 Frontend pod
kubectl exec -it deployment/issuegpt-frontend -n issuegpt -- /bin/sh

# 進入 MSSQL pod
kubectl exec -it mssql-0 -n issuegpt -- /bin/bash
```

---

## 常見問題

### Q: Pod 無法啟動

```bash
# 檢查事件
kubectl describe pod <pod-name> -n issuegpt

# 查看最近事件
kubectl get events -n issuegpt --sort-by='.lastTimestamp'
```

### Q: MSSQL 連接超時

```bash
# 檢查 MSSQL pod 是否就緒
kubectl get pod -l app=mssql -n issuegpt

# 查看 MSSQL 日誌
kubectl logs mssql-0 -n issuegpt | tail -20
```

### Q: API 無法連接到 MSSQL

```bash
# 進入 API pod 測試連接
kubectl exec -it <api-pod> -n issuegpt -- /bin/bash

# 在 pod 中：
curl -v telnet://mssql.issuegpt.svc.cluster.local:1433
```

### Q: Frontend 無法調用 API

- 檢查 nginx.conf 中的 proxy 設置
- 確認 CORS 已啟用
- 查看瀏覽器控制台 (F12) 的錯誤

### Q: 重新構建鏡像後部署沒有更新

```bash
# 刪除舊的 pod，K8s 會重建
kubectl delete pod -l app=issuegpt-api -n issuegpt
```

---

## 重啟與更新

### 重啟 Deployment

```bash
# 重啟 API
kubectl rollout restart deployment/issuegpt-api -n issuegpt

# 重啟 Frontend
kubectl rollout restart deployment/issuegpt-frontend -n issuegpt

# 查看更新狀態
kubectl rollout status deployment/issuegpt-api -n issuegpt
```

### 更新配置

```bash
# 編輯 ConfigMap
kubectl edit configmap issuegpt-config -n issuegpt

# 編輯 Secret
kubectl edit secret issuegpt-secrets -n issuegpt

# 然後重啟 pod
kubectl rollout restart deployment/issuegpt-api -n issuegpt
```

---

## 清理

### 刪除一切

```bash
# 刪除命名空間 (會刪除所有資源)
kubectl delete namespace issuegpt

# 清理本地 MSSQL 數據
rm -rf /tmp/mssql-data

# 刪除 Docker 鏡像 (可選)
docker rmi issuegpt-api:latest issuegpt-frontend:latest
```

### 使用清理腳本

```bash
chmod +x cleanup-k8s.sh
./cleanup-k8s.sh
```

---

## 性能優化 (可選)

### 增加副本數

```bash
# 編輯 API Deployment
kubectl edit deployment issuegpt-api -n issuegpt
# 修改 replicas: 2 → 3 或更高

# 編輯 Frontend Deployment
kubectl edit deployment issuegpt-frontend -n issuegpt
```

### 資源限制調整

```bash
# 編輯 api-deployment.yaml 的 resources 部分
# 或使用：
kubectl set resources deployment issuegpt-api -n issuegpt \
  --requests=cpu=250m,memory=512Mi \
  --limits=cpu=500m,memory=1Gi
```

---

## 生產部署參考

### 使用 Ingress

```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: issuegpt-ingress
  namespace: issuegpt
spec:
  rules:
    - host: issuegpt.example.com
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

### 推送到遠程倉庫

```bash
# 標記鏡像
docker tag issuegpt-api:latest registry.example.com/issuegpt-api:v1.0
docker tag issuegpt-frontend:latest registry.example.com/issuegpt-frontend:v1.0

# 推送
docker push registry.example.com/issuegpt-api:v1.0
docker push registry.example.com/issuegpt-frontend:v1.0

# 更新 YAML 中的 image 欄位
```

---

## 相關文件

- [`K8S_DEPLOYMENT.md`](K8S_DEPLOYMENT.md) — 詳細部署指南
- [`deploy-k8s.sh`](deploy-k8s.sh) — 自動部署腳本
- [`cleanup-k8s.sh`](cleanup-k8s.sh) — 清理腳本
- [`k8s/`](k8s/) — 所有 Kubernetes manifests

---

**IssueGPT 在 Kubernetes 上運行！** 🎉

有問題？查看 [`K8S_DEPLOYMENT.md`](K8S_DEPLOYMENT.md) 的完整故障排除指南。

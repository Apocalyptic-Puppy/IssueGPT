# ✅ IssueGPT Kubernetes 部署完成清單

## 📦 新增的 K8s 資源

### YAML 配置文件

- [x] `k8s/namespace.yaml` — issuegpt namespace
- [x] `k8s/configmap.yaml` — 環境配置
- [x] `k8s/secret.yaml` — 敏感信息 (GitHub Token, OpenAI Key)
- [x] `k8s/mssql-storage.yaml` — PersistentVolume & PersistentVolumeClaim
- [x] `k8s/mssql-statefulset.yaml` — MSSQL 部署
- [x] `k8s/api-deployment.yaml` — API 部署 (副本 × 2)
- [x] `k8s/frontend-deployment.yaml` — Frontend 部署 (副本 × 2)
- [x] `k8s/nginx.conf` — Nginx 配置

### Docker 構建文件

- [x] `Dockerfile.api` — ASP.NET Core 8.0 多階段構建
- [x] `Dockerfile.frontend` — Nginx + SPA 配置

### 部署腳本

- [x] `deploy-k8s.sh` — 一鍵自動部署 (含顏色輸出)
- [x] `cleanup-k8s.sh` — 清理所有資源

### 文檔指南

- [x] `K8S_DEPLOYMENT.md` — 完整部署指南 (故障排除)
- [x] `K8S_QUICK_START.md` — 3 分鐘快速開始
- [x] README.md 已更新 — 加入 K8s 選項

---

## 🎯 功能特性

### 本地 Kubernetes 支持

✅ Docker Desktop Kubernetes
✅ 本地 MSSQL 持久化存儲
✅ API 副本數 × 2 (負載均衡)
✅ Frontend 副本數 × 2 (高可用)
✅ 自動健康檢查 (liveness & readiness probes)

### 部署方式

✅ 自動構建 Docker 鏡像
✅ 一鍵部署所有資源
✅ 自動等待 Pod 就緒
✅ 完整的 CORS 配置

### Port-Forward 訪問

✅ 前端: `localhost:3000`
✅ API: `localhost:5000`
✅ MSSQL: `localhost:1433`

---

## 📊 K8s 架構

```
┌─────────────────────────────────────────┐
│        issuegpt Namespace               │
├─────────────────────────────────────────┤
│                                         │
│ Pod × 2 (API)          Pod × 2 (Frontend)
│ ├─ issuegpt-api-xxx    ├─ issuegpt-frontend-xxx
│ └─ issuegpt-api-yyy    └─ issuegpt-frontend-yyy
│                                         │
│ Service: issuegpt-api  Service: issuegpt-frontend
│ (ClusterIP: 80)        (ClusterIP: 80)
│                                         │
│ Pod × 1 (MSSQL)                         │
│ ├─ mssql-0 ────────────→ Storage (10Gi)│
│                                         │
│ Service: mssql (Headless)               │
│ (Port 1433)                             │
│                                         │
└─────────────────────────────────────────┘
```

---

## 🚀 使用方法

### 方法 1: 自動部署 (推薦)

```bash
chmod +x deploy-k8s.sh
./deploy-k8s.sh
```

### 方法 2: 手動部署

```bash
# 構建鏡像
docker build -f Dockerfile.api -t issuegpt-api:latest .
docker build -f Dockerfile.frontend -t issuegpt-frontend:latest .

# 逐步部署
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/mssql-storage.yaml
kubectl apply -f k8s/mssql-statefulset.yaml
kubectl apply -f k8s/api-deployment.yaml
kubectl apply -f k8s/frontend-deployment.yaml
```

### 方法 3: Port-Forward (3 個終端)

```bash
# 終端 1 - 前端
kubectl port-forward -n issuegpt svc/issuegpt-frontend 3000:80

# 終端 2 - API
kubectl port-forward -n issuegpt svc/issuegpt-api 5000:80

# 終端 3 - MSSQL (可選)
kubectl port-forward -n issuegpt svc/mssql 1433:1433
```

### 訪問應用

- **前端**: http://localhost:3000
- **API Swagger**: http://localhost:5000/swagger
- **MSSQL**: localhost:1433 (user: sa, password: IssueGPT@2026)

---

## 📋 K8s 資源清單

### StatefulSet

- [x] `mssql` — MSSQL Server with persistent storage

### Deployments

- [x] `issuegpt-api` — 2 replicas
- [x] `issuegpt-frontend` — 2 replicas

### Services

- [x] `mssql` (Headless ClusterIP)
- [x] `issuegpt-api` (ClusterIP)
- [x] `issuegpt-frontend` (ClusterIP)

### ConfigMaps

- [x] `issuegpt-config` — Environment variables

### Secrets

- [x] `issuegpt-secrets` — GitHub Token, OpenAI Key, DB Password

### Storage

- [x] `PersistentVolume` — Local /tmp/mssql-data
- [x] `PersistentVolumeClaim` — 10Gi for MSSQL

### Others

- [x] `Namespace` — issuegpt
- [x] `StorageClass` — local-storage

---

## 🛠️ 常用命令

### 使用 k9s 監控（推薦）

**安裝 k9s**

```bash
brew install k9s
```

**啟動 k9s**

```bash
# 全局視圖
k9s

# 只看 issuegpt 命名空間
k9s -n issuegpt
```

**快速導航快捷鍵**

| 快捷鍵    | 功能                        | 用途                    |
| --------- | --------------------------- | ----------------------- |
| `:po`     | 查看 Pods                   | 檢查 Pod 狀態和資源使用 |
| `:svc`    | 查看 Services               | 查看服務和端口映射      |
| `:dep`    | 查看 Deployments            | 檢查副本和更新狀態      |
| `:sts`    | 查看 StatefulSets           | 監控 MSSQL 狀態         |
| `:pvc`    | 查看 PersistentVolumeClaims | 檢查存儲使用            |
| `:cm`     | 查看 ConfigMaps             | 檢查環境變量            |
| `:events` | 查看事件日誌                | 故障診斷                |
| `l`       | 查看 Pod 日誌               | 實時日誌輸出            |
| `d`       | 查看詳情                    | 顯示完整資源信息        |
| `e`       | 編輯資源                    | 在線編輯配置            |
| `shift+f` | Port-forward                | 快速設置端口轉發        |
| `?`       | 幫助菜單                    | 查看所有快捷鍵          |
| `q`       | 退出                        | 返回上一層              |

**監控流程**

1. 啟動 k9s: `k9s -n issuegpt`
2. 按 `:po` 查看所有 Pods
3. 驗證狀態：
   - mssql-0: Ready 1/1
   - issuegpt-api-xxxxx: Ready 1/1
   - issuegpt-api-yyyyy: Ready 1/1
   - issuegpt-frontend-xxxxx: Ready 1/1
   - issuegpt-frontend-yyyyy: Ready 1/1
4. 選中 Pod 按 `l` 查看日誌
5. 按 `:dep` 檢查 Deployment 狀態 (Ready: 2/2)
6. 按 `:svc` 驗證 Services 運行正常

**資源使用情況檢查**

在 k9s Pod 列表中，查看 CPU 和 Memory 列：

- API Pod: CPU 100-250m (限制 500m), Memory 256-512Mi
- Frontend Pod: CPU 50-100m (限制 250m), Memory 64-128Mi
- MSSQL Pod: CPU 200-400m (限制 1000m), Memory 512-1024Mi

---

## 🛠️ kubectl 命令參考

### 查看狀態

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
```

### 進入 Pod

```bash
kubectl exec -it <pod-name> -n issuegpt -- /bin/bash
```

### 重啟 Deployment

```bash
kubectl rollout restart deployment/issuegpt-api -n issuegpt
kubectl rollout restart deployment/issuegpt-frontend -n issuegpt
```

### 清理

```bash
# 方法 1: 使用腳本
./cleanup-k8s.sh

# 方法 2: 手動
kubectl delete namespace issuegpt
rm -rf /tmp/mssql-data
```

---

## 📈 高可用性配置

### Pod 反親和性

API 和 Frontend 配置了 `podAntiAffinity`，保證副本在不同節點上運行。

### 健康檢查

- **Liveness Probe**: HTTP GET /swagger (API), / (Frontend)
- **Readiness Probe**: HTTP GET /swagger (API), / (Frontend)
- 自動重啟失敗的 Pod

### 資源限制

- API: CPU 250m-500m, Memory 512Mi-1Gi
- Frontend: CPU 100m-250m, Memory 64Mi-256Mi
- MSSQL: CPU 500m-1000m, Memory 2Gi-4Gi

---

## 🔒 安全性考量

- [x] Secrets 用於敏感信息 (GitHub Token, OpenAI Key)
- [x] CORS 設置允許跨域請求
- [x] 環境變量分離 (ConfigMap vs Secret)
- [ ] Ingress 帶 TLS (生產建議)
- [ ] RBAC 策略 (生產建議)
- [ ] Network Policies (生產建議)

---

## 📊 性能指標

| 組件     | CPU 請求 | CPU 限制 | 內存請求 | 內存限制 |
| -------- | -------- | -------- | -------- | -------- |
| API      | 250m     | 500m     | 512Mi    | 1Gi      |
| Frontend | 100m     | 250m     | 64Mi     | 256Mi    |
| MSSQL    | 500m     | 1000m    | 2Gi      | 4Gi      |

---

## 🚀 生產部署檢查表

- [ ] 推送 Docker 鏡像到容器倉庫 (DockerHub/ECR/ACR)
- [ ] 更新 image 欄位指向遠程倉庫
- [ ] 設置 Ingress 控制器
- [ ] 配置 TLS 證書
- [ ] 啟用 RBAC 策略
- [ ] 配置 Network Policies
- [ ] 設置 Resource Quotas
- [ ] 啟用 Monitoring (Prometheus/Grafana)
- [ ] 配置日誌聚合 (ELK/Loki)
- [ ] 設置備份策略 (MSSQL)

---

## 📚 相關文件

- [`K8S_DEPLOYMENT.md`](K8S_DEPLOYMENT.md) — 完整部署指南
- [`K8S_QUICK_START.md`](K8S_QUICK_START.md) — 快速開始
- [`deploy-k8s.sh`](deploy-k8s.sh) — 自動部署腳本
- [`cleanup-k8s.sh`](cleanup-k8s.sh) — 清理腳本
- [`Dockerfile.api`](Dockerfile.api) — API 鏡像
- [`Dockerfile.frontend`](Dockerfile.frontend) — Frontend 鏡像
- [`k8s/`](k8s/) — 所有 manifests

---

## ✅ 完成狀況

| 項目              | 狀態    |
| ----------------- | ------- |
| K8s Manifests     | ✅ 完成 |
| Docker Builds     | ✅ 完成 |
| 自動部署腳本      | ✅ 完成 |
| Port-Forward 支持 | ✅ 完成 |
| CORS 配置         | ✅ 完成 |
| 持久化存儲        | ✅ 完成 |
| 健康檢查          | ✅ 完成 |
| 文檔              | ✅ 完成 |

---

**IssueGPT 現在支持完整的 Kubernetes 部署！** 🚀

在 Mac 本地 Kubernetes 上，3 分鐘內即可部署並通過 port-forward 訪問。

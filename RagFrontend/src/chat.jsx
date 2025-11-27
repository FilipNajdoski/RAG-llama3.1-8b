import { useState, useRef, useEffect } from "react";

export default function Chat() {
  const [question, setQuestion] = useState("");
  const [messages, setMessages] = useState([]);
  const [loading, setLoading] = useState(false);
  const messagesEndRef = useRef(null);

  // Scroll to bottom when messages change
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const handleSend = async () => {
    // Don't send if input is empty or an existing request is in-flight
    if (loading || !question.trim()) return;

    const userMessage = { role: "user", text: question };
    setMessages((prev) => [...prev, userMessage]);
    setQuestion("");
    setLoading(true);

    try {
      const response = await fetch("https://localhost:7125/api/rag/query", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ question }),
      });

      const data = await response.json();
      const botMessage = { role: "bot", text: data.answer };
      setMessages((prev) => [...prev, botMessage]);
    } catch (err) {
      console.error(err);
      setMessages((prev) => [...prev, { role: "bot", text: "Error: failed to fetch answer." }]);
    } finally {
      setLoading(false);
    }
  };

  const handleKeyPress = (e) => {
    // Prevent sending new requests via Enter while an answer is pending
    if (e.key === "Enter" && !loading) handleSend();
  };

  return (
    <div className="chat-container">
      <div className="chat-header">Chatko</div>

      <div className="chat-messages">
        {messages.map((m, i) => (
          <div key={i} className={`message ${m.role}`}>
            <div className="message-text">{m.text}</div>
          </div>
        ))}
        {loading && (
          <div className="message bot">
            <div className="message-text">...</div>
          </div>
        )}
        <div ref={messagesEndRef} />
      </div>

      <div className="chat-input-container">
        <input
          type="text"
          value={question}
          onChange={(e) => setQuestion(e.target.value)}
          onKeyDown={handleKeyPress}
          placeholder="Type your question..."
        />
        <button
          onClick={handleSend}
          disabled={loading || !question.trim()}
          aria-disabled={loading ? "true" : "false"}
          title={loading ? "Waiting for answer..." : "Send"}
        >
          {loading ? "Sending…" : "Send"}
        </button>
      </div>
    </div>
  );
}

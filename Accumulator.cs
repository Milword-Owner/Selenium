using System;
using java.io;

namespace SpotifyPlugin
{
	// Token: 0x02000002 RID: 2
	public class Accumulator : DataOutputStream
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		internal Accumulator() : base(new ByteArrayOutputStream())
		{
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000205D File Offset: 0x0000025D
		internal virtual void dump()
		{
			this.bytes = ((ByteArrayOutputStream)this.@out).toByteArray();
			this.close();
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000207B File Offset: 0x0000027B
		internal virtual byte[] array()
		{
			return this.bytes;
		}

		// Token: 0x04000001 RID: 1
		private byte[] bytes;
	}
}
